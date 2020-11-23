import React, { useState, useEffect, useMemo } from "react"
import { css, Stack, Heading, Box, Button } from "bumbag"
import { useModalState } from "../../dialog/Modal"
import { useIsDisplayMenuByRoute } from "../../user/context"
import { Update as UpdateMenu } from "./menu"
import { EditDisclosure } from "../../dialog/Disclosure"
import FormDialog, { ConfirmButtonFooter } from "../../form/Dialog"
import { useUpdateProvider } from "./client"
import { Trans, useTrans } from "../../i18n"
import { FieldValues } from "../../layout/Grid"
import Label from "../../form/Label"
import { ToggleButtonField } from "./status"

export const EditButton = ({ record, onSubmitted, ...props }) => {
  let dialog = useModalState()
  let isDisplay = useIsDisplayMenuByRoute([UpdateMenu])
  if (!isDisplay) {
    return null
  }

  return (
    <>
      <EditDisclosure />
      {dialog.visible && (
        <EditDialog record={record} onSubmitted={onSubmitted} dialog={dialog} />
      )}
    </>
  )
}

const EditDialog = ({ record, onSubmitted, dialog }) => {
  let [submitData, setSubmit] = useState()
  let { result, isLoading, error } = useUpdateProvider(record?.id, submitData)

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
      onCloseErrorAlert={() => setSubmit(undefined)}
      onSubmitted={onSubmitted}
      title={title}
      showCloseButton
      dialog={dialog}
      footer={<ConfirmButtonFooter />}
    >
      <EditBody record={record} />
    </FormDialog>
  )
}

const EditBody = ({ record = {} }) => {
  let { providerName } = record
  return (
    <Stack orientation="vertical">
      <Heading use="h4">
        <Trans zh="基本资料" en="Basic Information" />
      </Heading>
      <FieldValues>
        <Label>
          <Trans zh="厂商" en="Vendor" />
        </Label>
        <ValueBlock></ValueBlock>
      </FieldValues>

      <Heading use="h4">
        <Trans zh="定期维护" en="Periodic Maintenance" />
      </Heading>
      <Label>
        <Trans zh="状态" en="Status" />
      </Label>
      <Box>
        <ToggleButtonField />
      </Box>

      <Label>
        <Trans zh="每周" en="Weekly" />
      </Label>
      <Box>TODO: input</Box>

      <Label>
        <Trans zh="时间" en="Time" />
      </Label>
      <Box>TODO: date range input with addon</Box>
    </Stack>
  )
}

const ValueBlock = ({ children, ...props }) => (
  <Text.Block alignY="center" {...props}>
    {children}
  </Text.Block>
)
