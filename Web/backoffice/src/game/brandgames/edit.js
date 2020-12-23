import React, { useState, useEffect, useMemo } from "react"
import { useFormikContext } from "formik"
import {
  css,
  Stack,
  Heading,
  Box,
  Button,
  Text,
  Group,
  Set,
  Flex
} from "bumbag"
import { useModalState } from "../../dialog/Modal"
import { useIsDisplayMenuByRoute } from "../../user/context"
import { Update as UpdateMenu } from "../providers/menu"
import { EditDisclosure } from "../../dialog/Disclosure"
import FormDialog, { ConfirmButtonFooter } from "../../form/Dialog"
import { useUpdateProvider } from "../providers/client"
import { Trans, useTrans } from "../../i18n"
import { FieldValues } from "../../layout/Grid"
import Label from "../../form/Label"

export const EditButton = ({ record, onSubmitted, ...props }) => {
  let dialog = useModalState()
  let isDisplay = useIsDisplayMenuByRoute([UpdateMenu])
  if (!isDisplay) {
    return null
  }

  return (
    <>
      <EditDisclosure {...dialog} />
      {dialog.visible && (
        <EditDialog record={record} onSubmitted={onSubmitted} dialog={dialog} />
      )}
    </>
  )
}

const EditDialog = ({ record, dialog, ...props }) => {
  let [submitData, setSubmit] = useState()
  let { result, isLoading, error, mutate, deleteCache } = useUpdateProvider(
    record?.id,
    submitData
  )

  const reset = () => {
    deleteCache()
    mutate(undefined, false)
    setSubmit(undefined)
  }

  let title = useTrans({
    zh: "编辑",
    en: "Edit"
  })
  return (
    <FormDialog
      initialValues={{ ...record }}
      onSubmit={setSubmit}
      result={result}
      isLoading={isLoading}
      error={error}
      onCloseErrorAlert={reset}
      onSubmitted={() => {
        reset()
        props?.onSubmitted()
      }}
      title={title}
      showCloseButton
      showActionButtons={false}
      dialog={dialog}
      footer={
        <ConfirmButtonFooter isLoading={isLoading} disabled={isLoading} />
      }
      maxWidth="unset"
    >
      <EditBody record={record} />
    </FormDialog>
  )
}

const EditBody = ({ record = {} }) => {
  let { name } = record
  let { values } = useFormikContext()
  return (
    <Stack orientation="vertical" width="500px">
      <Heading use="h5">
        <Trans zh="基本资料" en="Basic Information" />
      </Heading>
      <FieldValues>
        <Label>
          <Trans zh="厂商" en="Vendor" />
        </Label>
        <ValueBlock>{name}</ValueBlock>
      </FieldValues>

      <Heading use="h5">
        <Trans zh="特殊处理" en="Special Processing" />
      </Heading>
      <FieldValues>TODO</FieldValues>
    </Stack>
  )
}

const ValueBlock = ({ children, ...props }) => (
  <Text.Block alignY="center" {...props}>
    {children}
  </Text.Block>
)
