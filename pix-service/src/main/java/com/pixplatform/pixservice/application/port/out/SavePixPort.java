package com.pixplatform.pixservice.application.port.out;

import com.pixplatform.pixservice.domain.model.Pix;

public interface SavePixPort {
    Pix save(Pix pix);
}