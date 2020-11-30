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
import { Update as UpdateMenu } from "./menu"
import { EditDisclosure } from "../../dialog/Disclosure"
import FormDialog, { ConfirmButtonFooter } from "../../form/Dialog"
import { useUpdateProvider } from "./client"
import { Trans, useTrans } from "../../i18n"
import { FieldValues } from "../../layout/Grid"
import Label from "../../form/Label"
import { StatusMenuField } from "./status"
import { DaySelectField } from "../../date/week"
import { TimeSelectField } from "../../date/time"

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
        <Trans zh="定期维护" en="Periodic Maintenance" />
      </Heading>
      <FieldValues>
        <Label>
          <Trans zh="状态" en="Status" />
        </Label>
        <Box>
          <StatusMenuField name="status" size="small" />
        </Box>

        <Label>
          <Trans zh="每周" en="Weekly" />
        </Label>
        <Box>
          <DaySelectField name="day" />
        </Box>

        <Label>
          <Trans zh="开始时间" en="Start Time" />
        </Label>
        <Flex alignY="top" justifyContent="space-between">
          <TimeSelectField name="start" max={values?.end} />
          <Set orientation="horizontal">
            <Label marginRight="major-2">
              <Trans zh="结束时间" en="End Time" />
            </Label>
            <TimeSelectField name="end" min={values?.start} />
          </Set>
        </Flex>
      </FieldValues>
    </Stack>
  )
}

const ValueBlock = ({ children, ...props }) => (
  <Text.Block alignY="center" {...props}>
    {children}
  </Text.Block>
)
