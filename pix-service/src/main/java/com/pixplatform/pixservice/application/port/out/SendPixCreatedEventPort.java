package com.pixplatform.pixservice.application.port.out;

import com.pixplatform.pixservice.domain.model.Pix;

public interface SendPixCreatedEventPort {
    void send(Pix pix);
}