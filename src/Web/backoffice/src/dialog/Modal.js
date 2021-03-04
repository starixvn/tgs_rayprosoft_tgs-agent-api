import React, { useState, useEffect, useMemo, useRef, forwardRef } from "react"
import { Dialog, Modal as _Modal, Button } from "bumbag"
import Backdrop from "./Backdrop"
import { Trans } from "../i18n"
import { CloseText } from "../button/ActionButtons"

export const Modal = ({
    hideOnClickOutside = false,
    showCloseButton = true,
    hideBackdrop = false,
    finalFocusRef,
    children,
    ...props
}) => (
    <Backdrop {...props}>
        <Dialog.Modal
            hideOnClickOutside={hideOnClickOutside}
            showCloseButton={showCloseButton}
            unstable_finalFocusRef={finalFocusRef}
            {...props}
        >
            {children}
        </Dialog.Modal>
    </Backdrop>
)

export const useModalState = _Modal.useState

export const Footer = Dialog.Footer

//TODO: rm
export const CloseButtonFooter = ({ onClick, ...props }) => {
    return (
        <Footer {...props}>
            <Button type="button" onClick={onClick}>
                <CloseText />
            </Button>
        </Footer>
    )
}

export default Modal
