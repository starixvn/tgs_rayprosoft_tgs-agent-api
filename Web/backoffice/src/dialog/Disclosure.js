import React, { useState, useEffect, useMemo, useRef, forwardRef } from "react"
import { Button, Modal, css } from "bumbag"
import { Trans } from "../i18n"

export const Disclosure = forwardRef(
  ({ use = Button, children, ...props }, ref) => (
    <Modal.Disclosure
      ref={ref}
      use={use}
      css={css`
        flex: unset;
        white-space: nowrap;
      `}
      {...props}
    >
      {children}
    </Modal.Disclosure>
  )
)

export const EditDisclosure = forwardRef((props, ref) => (
  <Disclosure ref={ref} {...props}>
    <Trans zh="编辑" en="Edit" />
  </Disclosure>
))

export default Disclosure
