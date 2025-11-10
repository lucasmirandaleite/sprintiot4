package com.example.rfidtracking.service;

import com.example.rfidtracking.dto.RegistroRFIDDTO;
import com.example.rfidtracking.model.Moto;
import com.example.rfidtracking.model.RegistroRFID;
import com.example.rfidtracking.repository.MotoRepository;
import com.example.rfidtracking.repository.RegistroRFIDRepository;
import javax.persistence.EntityNotFoundException;
import org.modelmapper.ModelMapper;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.cache.annotation.CacheEvict;
import org.springframework.cache.annotation.CachePut;
import org.springframework.cache.annotation.Cacheable;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.Pageable;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.time.LocalDateTime;

@Service
public class RegistroRFIDService {

    private final RegistroRFIDRepository registroRFIDRepository;
    private final MotoRepository motoRepository;
    private final ModelMapper modelMapper;

    public RegistroRFIDService(RegistroRFIDRepository registroRFIDRepository, MotoRepository motoRepository, ModelMapper modelMapper) {
        this.registroRFIDRepository = registroRFIDRepository;
        this.motoRepository = motoRepository;
        this.modelMapper = modelMapper;
    }

    @Transactional(readOnly = true)
    @Cacheable(value = "registros", key = "#filtro + '_' + #pageable.pageNumber + '_' + #pageable.pageSize + '_' + #pageable.sort")
    public Page<RegistroRFIDDTO> listar(String filtro, Pageable pageable) {
        if (filtro != null && !filtro.isEmpty()) {
            return registroRFIDRepository.findByPontoLeituraContainingIgnoreCase(filtro, pageable)
                    .map(this::convertToDto);
        }
        return registroRFIDRepository.findAll(pageable).map(this::convertToDto);
    }

    @Transactional(readOnly = true)
    @Cacheable(value = "registro", key = "#id")
    public RegistroRFIDDTO buscarPorId(Long id) {
        RegistroRFID registro = registroRFIDRepository.findById(id)
                .orElseThrow(() -> new EntityNotFoundException("Registro RFID não encontrado com ID: " + id));
        return convertToDto(registro);
    }

    @Transactional
    @CacheEvict(value = "registros", allEntries = true) // Invalidate all paged results
    public RegistroRFIDDTO salvar(RegistroRFIDDTO dto) {
        RegistroRFID entity = new RegistroRFID();
        entity.setPontoLeitura(dto.getPontoLeitura());
        // DataHora should ideally be set by the DTO or based on business logic, setting to now() for simplicity
        entity.setDataHora(dto.getDataHora() != null ? dto.getDataHora() : LocalDateTime.now());


        Moto moto = motoRepository.findById(dto.getMotoId())
                .orElseThrow(() -> new EntityNotFoundException("Moto não encontrada com ID: " + dto.getMotoId()));
        entity.setMoto(moto);

        RegistroRFID registroSalvo = registroRFIDRepository.save(entity);
        return convertToDto(registroSalvo);
    }

    @Transactional
    @CachePut(value = "registro", key = "#id")
    @CacheEvict(value = "registros", allEntries = true)
    public RegistroRFIDDTO atualizar(Long id, RegistroRFIDDTO dto) {
        RegistroRFID registroExistente = registroRFIDRepository.findById(id)
                .orElseThrow(() -> new EntityNotFoundException("Registro RFID não encontrado com ID: " + id));

        registroExistente.setPontoLeitura(dto.getPontoLeitura());
        if (dto.getDataHora() != null) {
            registroExistente.setDataHora(dto.getDataHora());
        }

        if (dto.getMotoId() != null && (registroExistente.getMoto() == null || !dto.getMotoId().equals(registroExistente.getMoto().getId()))) {
            Moto moto = motoRepository.findById(dto.getMotoId())
                    .orElseThrow(() -> new EntityNotFoundException("Moto não encontrada com ID: " + dto.getMotoId()));
            registroExistente.setMoto(moto);
        }

        RegistroRFID registroAtualizado = registroRFIDRepository.save(registroExistente);
        return convertToDto(registroAtualizado);
    }

    @Transactional
    @CacheEvict(value = {"registro", "registros"}, allEntries = true) // Evict single and all paged results
    public void deletar(Long id) {
        if (!registroRFIDRepository.existsById(id)) {
            throw new EntityNotFoundException("Registro RFID não encontrado com ID: " + id);
        }
        registroRFIDRepository.deleteById(id);
    }

    private RegistroRFIDDTO convertToDto(RegistroRFID registroRFID) {
        RegistroRFIDDTO dto = modelMapper.map(registroRFID, RegistroRFIDDTO.class);
        if (registroRFID.getMoto() != null) {
            dto.setMotoId(registroRFID.getMoto().getId());
        }
        return dto;
    }

    // convertToEntity might not be strictly needed if mapping is simple and handled in save/update
    // private RegistroRFID convertToEntity(RegistroRFIDDTO dto) {
    //     return modelMapper.map(dto, RegistroRFID.class);
    // }
}

