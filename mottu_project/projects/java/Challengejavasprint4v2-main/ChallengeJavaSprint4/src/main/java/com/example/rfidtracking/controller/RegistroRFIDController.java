package com.example.rfidtracking.controller;

import com.example.rfidtracking.dto.RegistroRFIDDTO;
import com.example.rfidtracking.service.RegistroRFIDService;
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
@RequestMapping("/api/registros")
public class RegistroRFIDController {

    private final RegistroRFIDService registroRFIDService;

    public RegistroRFIDController(RegistroRFIDService registroRFIDService) {
        this.registroRFIDService = registroRFIDService;
    }

    @GetMapping
    public ResponseEntity<Page<RegistroRFIDDTO>> listar(
            @RequestParam(required = false, defaultValue = "") String filtro,
            Pageable pageable) {
        Page<RegistroRFIDDTO> pagina = registroRFIDService.listar(filtro, pageable);
        return ResponseEntity.ok(pagina);
    }

    @GetMapping("/{id}")
    public ResponseEntity<RegistroRFIDDTO> buscarPorId(@PathVariable Long id) {
        RegistroRFIDDTO dto = registroRFIDService.buscarPorId(id);
        return ResponseEntity.ok(dto);
    }

    @PostMapping
    public ResponseEntity<RegistroRFIDDTO> criar(@RequestBody @Valid RegistroRFIDDTO dto) {
        RegistroRFIDDTO registroSalvo = registroRFIDService.salvar(dto);
        URI location = ServletUriComponentsBuilder.fromCurrentRequest().path("/{id}")
                .buildAndExpand(registroSalvo.getId()).toUri();
        HttpHeaders headers = new HttpHeaders();
        headers.setLocation(location);
        return new ResponseEntity<>(registroSalvo, headers, HttpStatus.CREATED);
    }

    @PutMapping("/{id}")
    public ResponseEntity<RegistroRFIDDTO> atualizar(@PathVariable Long id, @RequestBody @Valid RegistroRFIDDTO dto) {
        RegistroRFIDDTO registroAtualizado = registroRFIDService.atualizar(id, dto);
        return ResponseEntity.ok(registroAtualizado);
    }

    @DeleteMapping("/{id}")
    public ResponseEntity<Void> deletar(@PathVariable Long id) {
        registroRFIDService.deletar(id);
        return ResponseEntity.noContent().build();
    }
}

