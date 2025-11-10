package com.example.rfidtracking.controller;

import com.example.rfidtracking.dto.RegistroRFIDDTO;
import com.example.rfidtracking.dto.MotoDTO;
import com.example.rfidtracking.service.RegistroRFIDService;
import com.example.rfidtracking.service.MotoService;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.PageRequest;
import org.springframework.data.domain.Pageable;
import org.springframework.data.domain.Sort;
import org.springframework.stereotype.Controller;
import org.springframework.ui.Model;
import org.springframework.validation.BindingResult;
import org.springframework.web.bind.annotation.*;
import org.springframework.web.servlet.mvc.support.RedirectAttributes;

import javax.validation.Valid;
import java.time.LocalDateTime;
import java.util.List;

@Controller
@RequestMapping("/registros")
public class RegistroRFIDWebController {

    private final RegistroRFIDService registroRFIDService;
    private final MotoService motoService;

    public RegistroRFIDWebController(RegistroRFIDService registroRFIDService, MotoService motoService) {
        this.registroRFIDService = registroRFIDService;
        this.motoService = motoService;
    }

    @GetMapping
    public String listar(
            @RequestParam(defaultValue = "0") int page,
            @RequestParam(defaultValue = "10") int size,
            @RequestParam(required = false) String filtro,
            @RequestParam(required = false) Long motoId,
            Model model) {
        
        Pageable pageable = PageRequest.of(page, size, Sort.by("dataHora").descending());
        Page<RegistroRFIDDTO> registros = registroRFIDService.listar(filtro != null ? filtro : "", pageable);
        
        // Se há filtro por moto, aplicar filtro adicional
        if (motoId != null) {
            List<RegistroRFIDDTO> registrosFiltrados = registros.getContent().stream()
                .filter(r -> r.getMotoId() != null && r.getMotoId().equals(motoId))
                .collect(java.util.stream.Collectors.toList());
            // Criar nova página com registros filtrados (simplificado)
            model.addAttribute("registros", new org.springframework.data.domain.PageImpl<>(registrosFiltrados, pageable, registrosFiltrados.size()));
        } else {
            model.addAttribute("registros", registros);
        }
        
        // Carregar lista de motos para o filtro
        carregarMotos(model);
        
        return "registros/list";
    }

    @GetMapping("/{id}")
    public String detalhar(@PathVariable Long id, Model model) {
        try {
            RegistroRFIDDTO registro = registroRFIDService.buscarPorId(id);
            model.addAttribute("registro", registro);
            
            // Buscar informações da moto
            if (registro.getMotoId() != null) {
                try {
                    MotoDTO moto = motoService.buscarPorId(registro.getMotoId());
                    model.addAttribute("moto", moto);
                    
                    // Buscar outros registros da mesma moto
                    Pageable pageable = PageRequest.of(0, 10, Sort.by("dataHora").descending());
                    Page<RegistroRFIDDTO> todosRegistros = registroRFIDService.listar("", pageable);
                    List<RegistroRFIDDTO> outrosRegistros = todosRegistros.getContent().stream()
                        .filter(r -> r.getMotoId() != null && r.getMotoId().equals(registro.getMotoId()))
                        .collect(java.util.stream.Collectors.toList());
                    model.addAttribute("outrosRegistros", outrosRegistros);
                } catch (Exception e) {
                    model.addAttribute("moto", null);
                }
            }
            
            return "registros/detail";
        } catch (Exception e) {
            return "redirect:/registros?error=Registro não encontrado";
        }
    }

    @GetMapping("/new")
    public String novoFormulario(@RequestParam(required = false) Long motoId, Model model) {
        RegistroRFIDDTO registro = new RegistroRFIDDTO();
        if (motoId != null) {
            registro.setMotoId(motoId);
        }
        registro.setDataHora(LocalDateTime.now());
        
        model.addAttribute("registro", registro);
        carregarMotos(model);
        return "registros/form";
    }

    @PostMapping
    public String criar(@Valid @ModelAttribute RegistroRFIDDTO registro, BindingResult result, Model model, RedirectAttributes redirectAttributes) {
        if (result.hasErrors()) {
            carregarMotos(model);
            return "registros/form";
        }
        
        try {
            // Se dataHora não foi informada, usar agora
            if (registro.getDataHora() == null) {
                registro.setDataHora(LocalDateTime.now());
            }
            
            RegistroRFIDDTO registroSalvo = registroRFIDService.salvar(registro);
            redirectAttributes.addFlashAttribute("success", "Registro RFID criado com sucesso!");
            return "redirect:/registros/" + registroSalvo.getId();
        } catch (Exception e) {
            model.addAttribute("error", "Erro ao criar registro: " + e.getMessage());
            carregarMotos(model);
            return "registros/form";
        }
    }

    @GetMapping("/{id}/edit")
    public String editarFormulario(@PathVariable Long id, Model model) {
        try {
            RegistroRFIDDTO registro = registroRFIDService.buscarPorId(id);
            model.addAttribute("registro", registro);
            carregarMotos(model);
            return "registros/form";
        } catch (Exception e) {
            return "redirect:/registros?error=Registro não encontrado";
        }
    }

    @PostMapping("/{id}")
    public String atualizar(@PathVariable Long id, @Valid @ModelAttribute RegistroRFIDDTO registro, BindingResult result, Model model, RedirectAttributes redirectAttributes) {
        if (result.hasErrors()) {
            registro.setId(id);
            carregarMotos(model);
            return "registros/form";
        }
        
        try {
            RegistroRFIDDTO registroAtualizado = registroRFIDService.atualizar(id, registro);
            redirectAttributes.addFlashAttribute("success", "Registro RFID atualizado com sucesso!");
            return "redirect:/registros/" + registroAtualizado.getId();
        } catch (Exception e) {
            model.addAttribute("error", "Erro ao atualizar registro: " + e.getMessage());
            registro.setId(id);
            carregarMotos(model);
            return "registros/form";
        }
    }

    @PostMapping("/{id}/delete")
    public String excluir(@PathVariable Long id, RedirectAttributes redirectAttributes) {
        try {
            registroRFIDService.deletar(id);
            redirectAttributes.addFlashAttribute("success", "Registro RFID excluído com sucesso!");
        } catch (Exception e) {
            redirectAttributes.addFlashAttribute("error", "Erro ao excluir registro: " + e.getMessage());
        }
        return "redirect:/registros";
    }

    private void carregarMotos(Model model) {
        try {
            Pageable pageable = PageRequest.of(0, 100, Sort.by("modelo"));
            Page<MotoDTO> motos = motoService.listar(pageable, null, null, null);
            model.addAttribute("motos", motos.getContent());
        } catch (Exception e) {
            model.addAttribute("motos", java.util.Collections.emptyList());
        }
    }
}
