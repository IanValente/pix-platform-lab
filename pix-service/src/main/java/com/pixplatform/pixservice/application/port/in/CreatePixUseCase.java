package com.pixplatform.pixservice.application.port.in;

import com.pixplatform.pixservice.domain.model.Pix;
import java.math.BigDecimal;

public interface CreatePixUseCase {
    Pix execute(String key, BigDecimal amount);
}