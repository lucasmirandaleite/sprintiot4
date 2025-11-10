package com.example.rfidtracking.service;

import com.example.rfidtracking.dto.MotoDTO;
import com.example.rfidtracking.model.Filial;
import com.example.rfidtracking.model.Moto;
import com.example.rfidtracking.repository.FilialRepository;
import com.example.rfidtracking.repository.MotoRepository;
import org.springframework.data.jpa.domain.Specification;
import javax.persistence.EntityNotFoundException;
import org.modelmapper.ModelMapper;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.Pageable;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

@Service
public class MotoService {

    private final MotoRepository motoRepository;
    private final FilialRepository filialRepository;
    private final ModelMapper modelMapper;

    public MotoService(MotoRepository motoRepository, FilialRepository filialRepository, ModelMapper modelMapper) {
        this.motoRepository = motoRepository;
        this.filialRepository = filialRepository;
        this.modelMapper = modelMapper;
    }

    @Transactional(readOnly = true)
    public Page<MotoDTO> listar(Pageable pageable, String modelo, String placa, String status) {
        Specification<Moto> spec = Specification.where(MotoSpecification.byModelo(modelo))
                .and(MotoSpecification.byPlaca(placa))
                .and(MotoSpecification.byStatus(status));

        return motoRepository.findAll(spec, pageable).map(this::convertToDto);
    }

    @Transactional(readOnly = true)
    public MotoDTO buscarPorId(Long id) {
        Moto moto = motoRepository.findById(id)
                .orElseThrow(() -> new EntityNotFoundException("Moto não encontrada com ID: " + id));
        return convertToDto(moto);
    }

    @Transactional
    public MotoDTO salvar(MotoDTO dto) {
        Moto moto = convertToEntity(dto);
            if (dto.getFilialId() != null) {
                moto.setFilial(buscarFilialPorId(dto.getFilialId()));
            }
        Moto motoSalva = motoRepository.save(moto);
        return convertToDto(motoSalva);
    }

    @Transactional
    public MotoDTO atualizar(Long id, MotoDTO dto) {
        Moto motoExistente = motoRepository.findById(id)
                .orElseThrow(() -> new EntityNotFoundException("Moto não encontrada com ID: " + id));

        // Update fields from DTO
        motoExistente.setModelo(dto.getModelo());
        motoExistente.setPlaca(dto.getPlaca());
        motoExistente.setStatus(dto.getStatus());

        if (dto.getFilialId() != null) {
            motoExistente.setFilial(buscarFilialPorId(dto.getFilialId()));
        } else {
            motoExistente.setFilial(null); // Or handle as per business logic if filialId is null
        }

        Moto motoAtualizada = motoRepository.save(motoExistente);
        return convertToDto(motoAtualizada);
    }

    @Transactional
    public void deletar(Long id) {
        if (!motoRepository.existsById(id)) {
            throw new EntityNotFoundException("Moto não encontrada com ID: " + id);
        }
        motoRepository.deleteById(id);
    }

    private MotoDTO convertToDto(Moto moto) {
        MotoDTO dto = modelMapper.map(moto, MotoDTO.class);
        if (moto.getFilial() != null) {
            dto.setFilialId(moto.getFilial().getIdFilial());
            dto.setNomeFilial(moto.getFilial().getNome()); // Populate filial name for DTO
        }
        return dto;
    }

    private Moto convertToEntity(MotoDTO dto) {
        return modelMapper.map(dto, Moto.class);
        // Note: Filial association is handled separately in save/update methods
    }

    private Filial buscarFilialPorId(Long filialId) {
        return filialRepository.findById(filialId)
                .orElseThrow(() -> new EntityNotFoundException("Filial não encontrada com ID: " + filialId));
    }
}

