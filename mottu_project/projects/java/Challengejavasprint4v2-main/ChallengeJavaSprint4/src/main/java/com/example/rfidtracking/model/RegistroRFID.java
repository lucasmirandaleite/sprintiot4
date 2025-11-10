
package com.example.rfidtracking.model;

import javax.persistence.*;
import javax.validation.constraints.NotBlank;
import java.time.LocalDateTime;

@Entity
public class RegistroRFID {
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    @NotBlank
    private String pontoLeitura;

    private LocalDateTime dataHora;

    @ManyToOne
    @JoinColumn(name = "moto_id", nullable = false)
    private Moto moto;

    public Long getId() { return id; }
    public void setId(Long id) { this.id = id; }
    public String getPontoLeitura() { return pontoLeitura; }
    public void setPontoLeitura(String pontoLeitura) { this.pontoLeitura = pontoLeitura; }
    public LocalDateTime getDataHora() { return dataHora; }
    public void setDataHora(LocalDateTime dataHora) { this.dataHora = dataHora; }
    public Moto getMoto() { return moto; }
    public void setMoto(Moto moto) { this.moto = moto; }

    @Override
    public boolean equals(Object o) {
        if (this == o) return true;
        if (o == null || getClass() != o.getClass()) return false;
        RegistroRFID that = (RegistroRFID) o;
        return id != null && id.equals(that.id);
    }

    @Override
    public int hashCode() {
        return 31;
    }
}
