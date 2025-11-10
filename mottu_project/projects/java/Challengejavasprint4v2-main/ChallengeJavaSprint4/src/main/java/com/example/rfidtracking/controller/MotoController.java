package com.example.rfidtracking.controller;

import com.example.rfidtracking.dto.MotoDTO;
import com.example.rfidtracking.service.MotoService;
import javax.validation.Valid;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.Pageable;
import org.springframework.http.HttpHeaders;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;
import org.springframework.web.servlet.support.ServletUriComponentsBuilder;

import java.net.URI;

@RestController
@RequestMapping("/api/motos")
public class MotoController {

    private final MotoService motoService;

    public MotoController(MotoService motoService) {
        this.motoService = motoService;
    }

    @GetMapping
    public ResponseEntity<Page<MotoDTO>> listar(
            Pageable pageable,
            @RequestParam(required = false) String modelo,
            @RequestParam(required = false) String placa,
            @RequestParam(required = false) String status) {
        Page<MotoDTO> pagina = motoService.listar(pageable, modelo, placa, status);
        return ResponseEntity.ok(pagina);
    }

    @GetMapping("/{id}")
    public ResponseEntity<MotoDTO> buscarPorId(@PathVariable Long id) {
        MotoDTO dto = motoService.buscarPorId(id);
        return ResponseEntity.ok(dto);
    }

    @PostMapping
    public ResponseEntity<MotoDTO> criar(@RequestBody @Valid MotoDTO dto) {
        MotoDTO motoSalva = motoService.salvar(dto);
        URI location = ServletUriComponentsBuilder.fromCurrentRequest().path("/{id}")
                .buildAndExpand(motoSalva.getId()).toUri();
        HttpHeaders headers = new HttpHeaders();
        headers.setLocation(location);
        return new ResponseEntity<>(motoSalva, headers, HttpStatus.CREATED);
    }

    @PutMapping("/{id}")
    public ResponseEntity<MotoDTO> atualizar(@PathVariable Long id, @RequestBody @Valid MotoDTO dto) {
        MotoDTO motoAtualizada = motoService.atualizar(id, dto);
        return ResponseEntity.ok(motoAtualizada);
    }

    @DeleteMapping("/{id}")
    public ResponseEntity<Void> deletar(@PathVariable Long id) {
        motoService.deletar(id);
        return ResponseEntity.noContent().build();
    }
}

