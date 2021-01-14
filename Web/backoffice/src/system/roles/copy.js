import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { useForm, FormProvider, useFormContext } from "react-hook-form"
import {
  Stack,
  Flex,
  Box,
  Card,
  Set,
  Button,
  Text,
  Label,
  Dialog,
  Modal,
  useClassName
} from "bumbag"
import LoadingBackdrop from "../../dialog/LoadingBackdrop"
import AlertDialog from "../../dialog/Alert"
import { Copy as CopyMenu } from "./menu"
import { useIsDisplayMenuByRoute } from "../../user/context"
import Disclosure from "../../dialog/Disclosure"
import { useCopyRole } from "./client"
import { Trans, useTrans } from "../../i18n"
import FormDialog from "../../form/FormDialog"
import Input from "../../input/Input"

export const CopyButton = ({ id, name, onSubmitted, disabled, ...props }) => {
  let dialog = Modal.useState()
  let disclosureRef = useRef()

  let isDisplay = useIsDisplayMenuByRoute([CopyMenu])
  if (!isDisplay) {
    return null
  }

  return (
    <>
      <Disclosure disabled={disabled} {...dialog}>
        <Trans zh="复制" en="Copy" />
      </Disclosure>
      {dialog.visible && (
        <CopyDialog
          id={id}
          name={name}
          onSubmitted={onSubmitted}
          dialog={dialog}
          finalFocusRef={disclosureRef}
        />
      )}
    </>
  )
}

const CopyDialog = ({ id, name = "", onSubmitted, dialog, ...props }) => {
  let [submitData, setSubmit] = useState()
  let { result, error, mutate, isLoading } = useCopyRole(id, submitData)

  const reset = () => {
    setSubmit(undefined)
    //mutate()
  }

  let title = useTrans({
    zh: "复制权限组",
    en: "Copy Role"
  })

  return (
    <FormDialog
      defaultValues={{
        name
      }}
      onSubmit={setSubmit}
      isLoading={isLoading}
      result={result}
      error={error}
      onCloseErrorAlert={reset}
      onSubmitted={onSubmitted}
      title={title}
      dialog={dialog}
    >
      <Input name="name" required />
    </FormDialog>
  )
}
