import React, { useState, useEffect, useMemo, useRef, forwardRef } from "react"
import { Button, Modal, css } from "bumbag"

export const Disclosure = forwardRef(
  ({ use = Button, children, ...props }, ref) => (
    <Modal.Disclosure
      ref={ref}
      use={use}
      css={css`
        flex: unset;
      `}
      {...props}
    >
      {children}
    </Modal.Disclosure>
  )
)

export default Disclosure
