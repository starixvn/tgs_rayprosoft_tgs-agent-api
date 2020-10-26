import React, { useState, useEffect, useMemo, useRef, forwardRef } from "react"
import { Dialog, Modal as _Modal } from "bumbag"
import Backdrop from "./Backdrop"

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

export default Modal
