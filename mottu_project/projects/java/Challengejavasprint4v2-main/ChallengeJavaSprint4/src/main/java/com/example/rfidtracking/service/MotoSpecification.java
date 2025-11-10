package com.example.rfidtracking.service;

import com.example.rfidtracking.model.Moto;
import org.springframework.data.jpa.domain.Specification;

public class MotoSpecification {

    public static Specification<Moto> byModelo(String modelo) {
        return (root, query, builder) -> {
            if (modelo == null || modelo.isEmpty()) {
                return builder.conjunction(); // Retorna uma condição verdadeira (sem filtro)
            }
            return builder.like(builder.lower(root.get("modelo")), "%" + modelo.toLowerCase() + "%");
        };
    }

    public static Specification<Moto> byPlaca(String placa) {
        return (root, query, builder) -> {
            if (placa == null || placa.isEmpty()) {
                return builder.conjunction();
            }
            return builder.like(builder.lower(root.get("placa")), "%" + placa.toLowerCase() + "%");
        };
    }

    public static Specification<Moto> byStatus(String status) {
        return (root, query, builder) -> {
            if (status == null || status.isEmpty()) {
                return builder.conjunction();
            }
            return builder.like(builder.lower(root.get("status")), "%" + status.toLowerCase() + "%");
        };
    }
}
