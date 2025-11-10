package com.example.rfidtracking.repository;

import com.example.rfidtracking.model.RegistroRFID;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.Pageable;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

@Repository
public interface RegistroRFIDRepository extends JpaRepository<RegistroRFID, Long> {
    Page<RegistroRFID> findByPontoLeituraContainingIgnoreCase(String filtro, Pageable pageable);
}

