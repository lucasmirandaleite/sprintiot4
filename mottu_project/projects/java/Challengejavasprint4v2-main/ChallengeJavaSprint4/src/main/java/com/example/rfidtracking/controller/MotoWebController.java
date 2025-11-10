package com.example.rfidtracking.controller;

import com.example.rfidtracking.dto.MotoDTO;
import com.example.rfidtracking.dto.FilialDTO;
import com.example.rfidtracking.dto.RegistroRFIDDTO;
import com.example.rfidtracking.service.MotoService;
import com.example.rfidtracking.service.FilialService;
import com.example.rfidtracking.service.RegistroRFIDService;
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
import java.util.List;

@Controller
@RequestMapping("/motos")
public class MotoWebController {

        private final MotoService motoService;
    private final FilialService filialService;
    private final RegistroRFIDService registroRFIDService;

    public MotoWebController(MotoService motoService, FilialService filialService, RegistroRFIDService registroRFIDService) {
        this.motoService = motoService;
        this.filialService = filialService;
        this.registroRFIDService = registroRFIDService;
    }

    @GetMapping
    public String listar(
            @RequestParam(defaultValue = "0") int page,
            @RequestParam(defaultValue = "10") int size,
            @RequestParam(required = false) String modelo,
            @RequestParam(required = false) String placa,
            @RequestParam(required = false) String status,
            Model model) {
        
        Pageable pageable = PageRequest.of(page, size, Sort.by("id").descending());
        Page<MotoDTO> motos = motoService.listar(pageable, modelo, placa, status);
        
        model.addAttribute("motos", motos);
        return "motos/list";
    }

    @GetMapping("/{id}")
    public String detalhar(@PathVariable Long id, Model model) {
        try {
            MotoDTO moto = motoService.buscarPorId(id);
            model.addAttribute("moto", moto);
            
            // Buscar registros RFID da moto
            try {
                Pageable pageable = PageRequest.of(0, 10, Sort.by("dataHora").descending());
                Page<RegistroRFIDDTO> registros = registroRFIDService.listar("", pageable);
                // Filtrar registros da moto específica
                List<RegistroRFIDDTO> registrosDaMoto = registros.getContent().stream()
                    .filter(r -> r.getMotoId() != null && r.getMotoId().equals(id))
                    .collect(java.util.stream.Collectors.toList());
                model.addAttribute("registros", registrosDaMoto);
            } catch (Exception e) {
                model.addAttribute("registros", java.util.Collections.emptyList());
            }
            
            return "motos/detail";
        } catch (Exception e) {
            return "redirect:/motos?error=Moto não encontrada";
        }
    }

    @GetMapping("/new")
    public String novoFormulario(Model model) {
        model.addAttribute("moto", new MotoDTO());
        carregarFiliais(model);
        return "motos/form";
    }

    @PostMapping
    public String criar(@Valid @ModelAttribute MotoDTO moto, BindingResult result, Model model, RedirectAttributes redirectAttributes) {
        if (result.hasErrors()) {
            carregarFiliais(model);
            return "motos/form";
        }
        
        try {
            MotoDTO motoSalva = motoService.salvar(moto);
            redirectAttributes.addFlashAttribute("success", "Moto criada com sucesso!");
            return "redirect:/motos/" + motoSalva.getId();
        } catch (Exception e) {
            model.addAttribute("error", "Erro ao criar moto: " + e.getMessage());
            carregarFiliais(model);
            return "motos/form";
        }
    }

    @GetMapping("/{id}/edit")
    public String editarFormulario(@PathVariable Long id, Model model) {
        try {
            MotoDTO moto = motoService.buscarPorId(id);
            model.addAttribute("moto", moto);
            carregarFiliais(model);
            return "motos/form";
        } catch (Exception e) {
            return "redirect:/motos?error=Moto não encontrada";
        }
    }

    @PostMapping("/{id}")
    public String atualizar(@PathVariable Long id, @Valid @ModelAttribute MotoDTO moto, BindingResult result, Model model, RedirectAttributes redirectAttributes) {
        if (result.hasErrors()) {
            moto.setId(id);
            carregarFiliais(model);
            return "motos/form";
        }
        
        try {
            MotoDTO motoAtualizada = motoService.atualizar(id, moto);
            redirectAttributes.addFlashAttribute("success", "Moto atualizada com sucesso!");
            return "redirect:/motos/" + motoAtualizada.getId();
        } catch (Exception e) {
            model.addAttribute("error", "Erro ao atualizar moto: " + e.getMessage());
            moto.setId(id);
            carregarFiliais(model);
            return "motos/form";
        }
    }

    @PostMapping("/{id}/delete")
    public String excluir(@PathVariable Long id, RedirectAttributes redirectAttributes) {
        try {
            motoService.deletar(id);
            redirectAttributes.addFlashAttribute("success", "Moto excluída com sucesso!");
        } catch (Exception e) {
            redirectAttributes.addFlashAttribute("error", "Erro ao excluir moto: " + e.getMessage());
        }
        return "redirect:/motos";
    }

    private void carregarFiliais(Model model) {
        try {
            List<FilialDTO> filiais = filialService.listarTodas();
            model.addAttribute("filiais", filiais);
        } catch (Exception e) {
            model.addAttribute("filiais", java.util.Collections.emptyList());
        }
    }
}
