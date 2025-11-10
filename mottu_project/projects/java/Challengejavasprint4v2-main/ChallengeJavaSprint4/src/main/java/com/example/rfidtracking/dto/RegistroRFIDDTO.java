package com.example.rfidtracking.dto;

import javax.validation.constraints.NotBlank;
import java.time.LocalDateTime; // Import LocalDateTime
import javax.validation.constraints.NotNull;

public class RegistroRFIDDTO {
    private Long id;

    @NotBlank
    private String pontoLeitura;

    private LocalDateTime dataHora; // Added dataHora field

    @NotNull(message = "A Moto deve ser selecionada.")
    private Long motoId;

    // Getters and Setters
    public Long getId() { 
        return id; 
    }

    public void setId(Long id) { 
        this.id = id; 
    }

    public String getPontoLeitura() { 
        return pontoLeitura; 
    }

    public void setPontoLeitura(String pontoLeitura) { 
        this.pontoLeitura = pontoLeitura; 
    }

    public LocalDateTime getDataHora() { // Added getDataHora
        return dataHora;
    }

    public void setDataHora(LocalDateTime dataHora) { // Added setDataHora
        this.dataHora = dataHora;
    }

    public Long getMotoId() { 
        return motoId; 
    }

    public void setMotoId(Long motoId) { 
        this.motoId = motoId; 
    }
}

