package com.example.rfidtracking.service;

import com.example.rfidtracking.dto.FilialDTO;
import com.example.rfidtracking.model.Filial;
import com.example.rfidtracking.repository.FilialRepository;
import org.modelmapper.ModelMapper;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.util.List;
import java.util.stream.Collectors;

@Service
public class FilialService {

    private final FilialRepository filialRepository;
    private final ModelMapper modelMapper;

    public FilialService(FilialRepository filialRepository, ModelMapper modelMapper) {
        this.filialRepository = filialRepository;
        this.modelMapper = modelMapper;
    }

    @Transactional(readOnly = true)
    public List<FilialDTO> listarTodas() {
        List<Filial> filiais = filialRepository.findAll();
        return filiais.stream()
                .map(this::convertToDto)
                .collect(Collectors.toList());
    }

    @Transactional(readOnly = true)
    public FilialDTO buscarPorId(Long id) {
        Filial filial = filialRepository.findById(id)
                .orElseThrow(() -> new RuntimeException("Filial não encontrada com ID: " + id));
        return convertToDto(filial);
    }

    @Transactional
    public FilialDTO salvar(FilialDTO dto) {
        Filial filial = convertToEntity(dto);
        Filial filialSalva = filialRepository.save(filial);
        return convertToDto(filialSalva);
    }

    @Transactional
    public FilialDTO atualizar(Long id, FilialDTO dto) {
        Filial filialExistente = filialRepository.findById(id)
                .orElseThrow(() -> new RuntimeException("Filial não encontrada com ID: " + id));

        filialExistente.setNome(dto.getNome());
        filialExistente.setCidade(dto.getCidade());
        filialExistente.setEstado(dto.getEstado());

        Filial filialAtualizada = filialRepository.save(filialExistente);
        return convertToDto(filialAtualizada);
    }

    @Transactional
    public void deletar(Long id) {
        if (!filialRepository.existsById(id)) {
            throw new RuntimeException("Filial não encontrada com ID: " + id);
        }
        filialRepository.deleteById(id);
    }

    private FilialDTO convertToDto(Filial filial) {
        return modelMapper.map(filial, FilialDTO.class);
    }

    private Filial convertToEntity(FilialDTO dto) {
        return modelMapper.map(dto, Filial.class);
    }
}
