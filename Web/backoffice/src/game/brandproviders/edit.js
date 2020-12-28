import React, { useState, useEffect, useMemo } from "react"
import { useForm, useFormContext } from "react-hook-form"
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
import { useUpdateBrandGameProvider } from "./client"
import { Trans, useTrans } from "../../i18n"
import { FieldValues } from "../../layout/Grid"
import Label from "../../form/Label"
import { WeekDaySelect } from "../../date/week"
import {
  formatHourMinute,
  TimeSelectField,
  hourMinuteToDate
} from "../../date/time"
import HookFormDialog from "../../form/HookFormDialog"
import parseDate from "date-fns/fp/parseISO"
import { MaintenanceStatusToggleButton } from "./ScheduleStatus"

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
  let {
    result,
    isLoading,
    error,
    mutate,
    deleteCache
  } = useUpdateBrandGameProvider(record?.id, submitData)

  const onSubmit = ({ from, to, ...formData }) => {
    from = hourMinuteToDate(from)
    to = hourMinuteToDate(to)
    setSubmit({
      ...formData,
      from,
      to
    })
  }

  const reset = () => {
    deleteCache()
    mutate(undefined, false)
    setSubmit(undefined)
  }

  let title = useTrans({
    zh: "编辑",
    en: "Edit"
  })

  let from = formatHourMinute(parseDate(record.from))
  let to = formatHourMinute(parseDate(record.to))

  return (
    <HookFormDialog
      defaultValues={{
        ...record,
        from,
        to
      }}
      onSubmit={onSubmit}
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
    </HookFormDialog>
  )
}

const EditBody = ({ record = {} }) => {
  let { gameProviderName } = record
  let { watch } = useFormContext()
  let from = watch("from")
  let to = watch("to")

  return (
    <Stack orientation="vertical" width="500px">
      <Heading use="h5">
        <Trans zh="基本资料" en="Basic Information" />
      </Heading>
      <FieldValues>
        <Label>
          <Trans zh="厂商" en="Vendor" />
        </Label>
        <ValueBlock>{gameProviderName}</ValueBlock>
      </FieldValues>

      <Heading use="h5">
        <Trans zh="定期维护" en="Periodic Maintenance" />
      </Heading>
      <FieldValues>
        <Label>
          <Trans zh="状态" en="Status" />
        </Label>
        <Box>
          <MaintenanceStatusToggleButton name="scheduleStatus" required />
        </Box>

        <Label>
          <Trans zh="每周" en="Weekly" />
        </Label>
        <Box>
          <WeekDaySelect name="dayOfWeek" required />
        </Box>

        <Label>
          <Trans zh="开始时间" en="Start Time" />
        </Label>
        <Flex alignY="top" justifyContent="space-between">
          <TimeSelectField name="from" required max={to} />
          <Set orientation="horizontal">
            <Label marginRight="major-2">
              <Trans zh="结束时间" en="End Time" />
            </Label>
            <TimeSelectField name="to" required min={from} />
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
