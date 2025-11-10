package com.example.rfidtracking.security;

import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.security.config.annotation.web.builders.HttpSecurity;
import org.springframework.security.config.annotation.web.configuration.EnableWebSecurity;
import org.springframework.security.core.userdetails.UserDetailsService;
import org.springframework.security.provisioning.JdbcUserDetailsManager;
import javax.sql.DataSource;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.security.web.SecurityFilterChain;
import org.springframework.security.crypto.bcrypt.BCryptPasswordEncoder;
import org.springframework.security.crypto.password.PasswordEncoder; // <-- NOVO IMPORT

@Configuration
@EnableWebSecurity
public class SecurityConfig {

    @Bean
    public SecurityFilterChain securityFilterChain(HttpSecurity http ) throws Exception {
        http
     .csrf().ignoringAntMatchers("/api/**", "/h2-console/**").and()
            .headers().frameOptions().sameOrigin().and()
            .authorizeRequests()
                .antMatchers("/**").permitAll();
        return http.build( );
    }

   	    @Bean
	    public UserDetailsService userDetailsService(DataSource dataSource) {
	        JdbcUserDetailsManager users = new JdbcUserDetailsManager(dataSource);
	        // Configura o JdbcUserDetailsManager para usar a tabela 'usuario' e as colunas corretas
	        users.setUsersByUsernameQuery("select username, password, true from usuario where username = ?");
	        users.setAuthoritiesByUsernameQuery("select username, role from usuario where username = ?");     return users;
	    }

    // NOVO MÉTODO: Define o codificador de senha como NoOp (sem criptografia)
    @Bean
    public PasswordEncoder passwordEncoder() {
        return new BCryptPasswordEncoder();
    }
}
