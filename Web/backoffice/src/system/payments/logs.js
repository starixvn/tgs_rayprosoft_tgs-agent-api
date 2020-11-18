import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/core"
import { Formik, Field, Form, useFormikContext } from "formik"
import { Stack, Box, Button, Text, Modal, Dialog } from "bumbag"
import { useModalState } from "../../dialog/Modal"
import { useIsDisplayMenuByRoute } from "../../user/context"
import { View as ViewMenu } from "./menu"
import { Trans, useTrans } from "../../i18n"
import Disclosure from "../../dialog/Disclosure"

export const ViewButton = ({ record, ...props }) => {
  let dialog = useModalState()

  let isDisplay = useIsDisplayMenuByRoute([ViewMenu])
  if (!isDisplay) {
    return null
  }

  return (
    <>
      <Disclosure {...dialog}>
        <Trans zh="日志" en="Logs" />
      </Disclosure>
      {dialog.visible && <LogsDialog record={record} dialog={dialog} />}
    </>
  )
}

const LogsDialog = ({ record, dialog, ...props }) => {
  //TODO: showCloseButton
  return (
    <Dialog.Modal
      title=""
      showCloseButton
      hideOnClickOutside={false}
      {...dialog}
    >
      TODO: logs table
    </Dialog.Modal>
  )
}
