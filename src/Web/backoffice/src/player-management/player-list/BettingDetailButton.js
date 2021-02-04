import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { useLocation, useRouteMatch } from "react-router-dom"
import { Stack, Box, Text, Dialog } from "bumbag"
import Disclosure from "../../dialog/Disclosure"
import { Modal, useModalState } from "../../dialog/Modal"
import { Trans, useTrans } from "../../i18n"

export const Button = ({ record, ...props }) => {
  let dialog = useModalState()
  let disclosureRef = useRef()

  return (
    <>
      <Disclosure ref={disclosureRef} {...dialog}>
        <Trans zh="详情" en="Detail" />
      </Disclosure>
      <Modal
        title={useTrans(titleTrans)}
        finalFocusRef={disclosureRef}
        {...dialog}
      >
        TODO: {JSON.stringify(record)}
      </Modal>
    </>
  )
}

const titleTrans = {
  zh: "详情",
  en: "Detail"
}

export default Button
