import React, { useState, useEffect, useMemo } from "react"
import { css, Global } from "@emotion/react"
import { Modal } from "bumbag"

const BumbagZIndex = 19900410

export const Backdrop = ({ zIndex = BumbagZIndex, children, ...props }) => {
  let zIndexString = `${zIndex + 100}`
  return (
    <Modal.Backdrop zIndex={zIndexString} {...props}>
      {children}
    </Modal.Backdrop>
  )
}

export default Backdrop
