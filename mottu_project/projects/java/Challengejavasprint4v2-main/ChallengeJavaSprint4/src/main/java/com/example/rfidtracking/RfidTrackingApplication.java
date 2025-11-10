
package com.example.rfidtracking;

import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.cache.annotation.EnableCaching;

@SpringBootApplication
@EnableCaching
public class RfidTrackingApplication {
    public static void main(String[] args) {
        SpringApplication.run(RfidTrackingApplication.class, args);
    }
}
