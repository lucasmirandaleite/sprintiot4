package com.example.rfidtracking.dto;

import javax.validation.constraints.NotBlank;

public class MotoDTO {
    private Long id;

    @NotBlank
    private String modelo;

    @NotBlank
    private String placa;

    private String status; // Added status

    private Long filialId; // Added to represent the Filial's ID
    private String nomeFilial; // Added to represent the Filial's name (optional, for read operations)


    // Getters and Setters
    public Long getId() {
        return id;
    }

    public void setId(Long id) {
        this.id = id;
    }

    public String getModelo() {
        return modelo;
    }

    public void setModelo(String modelo) {
        this.modelo = modelo;
    }

    public String getPlaca() {
        return placa;
    }

    public void setPlaca(String placa) {
        this.placa = placa;
    }

    public String getStatus() {
        return status;
    }

    public void setStatus(String status) {
        this.status = status;
    }

    public Long getFilialId() {
        return filialId;
    }

    public void setFilialId(Long filialId) {
        this.filialId = filialId;
    }

    public String getNomeFilial() {
        return nomeFilial;
    }

    public void setNomeFilial(String nomeFilial) {
        this.nomeFilial = nomeFilial;
    }
}

