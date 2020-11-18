import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/core"
import { Formik, Field, Form, useFormikContext } from "formik"
import { Stack, Box, Button, Text, Modal, Dialog } from "bumbag"
import { useModalState } from "../../dialog/Modal"
import { useIsDisplayMenuByRoute } from "../../user/context"
import { Update as UpdateMenu } from "./menu"
import { Trans, useTrans } from "../../i18n"
import Disclosure from "../../dialog/Disclosure"
import FormDialog from "../../form/Dialog"

export const EditButton = ({ record, onSubmitted, ...props }) => {
  let dialog = useModalState()

  let isDisplay = useIsDisplayMenuByRoute([UpdateMenu])
  if (!isDisplay) {
    return null
  }

  return (
    <>
      <Disclosure {...dialog}>
        <Trans zh="手续费" en="Handling Fee" />
      </Disclosure>
      {dialog.visible && <EditDialog record={record} dialog={dialog} />}
    </>
  )
}

//TODO:?
export const ViewButton = ({}) => {}

const EditDialog = ({ record, dialog, ...props }) => {
  //TODO: custom footer
  let title = useTrans({
    zh: "手续费",
    en: "Handling Fee"
  })
  return (
    <FormDialog initialValues={{}} title={title} showCloseButton {...dialog}>
      <EditBody />
    </FormDialog>
  )
}

const EditBody = ({}) => {
  return <div>TODO: 2 cols</div>
}
