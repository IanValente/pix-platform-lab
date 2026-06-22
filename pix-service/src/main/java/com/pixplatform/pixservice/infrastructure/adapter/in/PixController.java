package com.pixplatform.pixservice.infrastructure.adapter.in;

import com.pixplatform.pixservice.application.port.in.CreatePixUseCase;
import com.pixplatform.pixservice.domain.model.Pix;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.math.BigDecimal;

@RestController
@RequestMapping("/api/v1/pix")
@CrossOrigin(origins = "*")
public class PixController {

    // O Controller conhece apenas a PORTA (interface), não a implementação. Desacoplamento perfeito!
    private final CreatePixUseCase createPixUseCase;

    public PixController(CreatePixUseCase createPixUseCase) {
        this.createPixUseCase = createPixUseCase;
    }

    @PostMapping
    public ResponseEntity<PixResponse> createPix(@RequestBody PixRequest request) {
        Pix pix = createPixUseCase.execute(request.key(), request.amount());

        PixResponse response = new PixResponse(pix.getId().toString(), pix.getStatus().name());
        return ResponseEntity.ok(response);
    }
}

// DTOs em formato de Record (novidade do Java 14+ para código mais limpo)
record PixRequest(String key, BigDecimal amount) {}
record PixResponse(String transactionId, String status) {}