import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { Stack, Box, Button, Text, Modal, Dialog, Heading } from "bumbag"
import { useModalState } from "../../dialog/Modal"
import { useIsDisplayMenuByRoute } from "../../user/context"
import { Update as UpdateMenu } from "./menu"
import { Trans, useTrans } from "../../i18n"
import Disclosure from "../../dialog/Disclosure"
import { ConfirmButtonFooter } from "../../form/Dialog"
import { useUpdateBrandPaymentHandingFee } from "./client"
import { FieldValues } from "../../layout/Grid"
import { NormalLabel as Label } from "../../form/Label"
import HookFormInputField from "../../input/InputField"
import HookFormDialog from "../../form/HookFormDialog"

const HandlingFeeTrans = {
  zh: "手续费",
  en: "Handling fee"
}

export const EditButton = ({ record, onSubmitted, ...props }) => {
  let dialog = useModalState()

  let isDisplay = useIsDisplayMenuByRoute([UpdateMenu])
  if (!isDisplay) {
    return null
  }

  return (
    <>
      <Disclosure {...dialog}>
        <Trans {...HandlingFeeTrans} />
      </Disclosure>
      {dialog.visible && (
        <EditDialog record={record} onSubmitted={onSubmitted} dialog={dialog} />
      )}
    </>
  )
}

const EditDialog = ({ record = {}, dialog, onSubmitted, ...props }) => {
  let title = useTrans({
    zh: "手续费",
    en: "Handling Fee"
  })

  let [submitData, setSubmit] = useState()
  let {
    result,
    isLoading,
    error,
    mutate,
    deleteCache
  } = useUpdateBrandPaymentHandingFee(record?.id, submitData)

  const reset = () => {
    deleteCache()
    mutate(undefined, false)
    setSubmit(undefined)
  }

  return (
    <HookFormDialog
      defaultValues={record.paymentFee}
      onSubmit={setSubmit}
      result={result}
      isLoading={isLoading}
      error={error}
      onCloseErrorAlert={reset}
      onSubmitted={onSubmitted}
      title={title}
      showCloseButton
      showActionButtons={false}
      footer={
        <ConfirmButtonFooter
          alignX="center"
          width="100%"
          isLoading={isLoading}
          disabled={isLoading}
        />
      }
      dialog={dialog}
    >
      <EditBody record={record} dialog={dialog} />
    </HookFormDialog>
  )
}

const EditBody = ({ record = {}, dialog, ...props }) => {
  return (
    <Stack orientation="vertical" spacing="minor-1">
      <Heading use="h5">
        <Trans zh="基本资料" en="Basic Information" />
      </Heading>
      <FieldValues>
        <Label>
          <Trans zh="支付厂商" en="Payment Vendor" />
        </Label>
        <ValueBlock>{record?.paymentVendorName}</ValueBlock>

        <Label>
          <Trans zh="支付方式" en="Payment Medthod" />
        </Label>
        <ValueBlock>{record?.paymentMethodName}</ValueBlock>

        <Label>
          <Trans zh="支付通道" en="Payment Channel" />
        </Label>
        <ValueBlock>{record?.paymentChannelName}</ValueBlock>

        <Label required>
          <Trans {...HandlingFeeTrans} />
        </Label>
        <Box>
          <HookFormInputField
            name="feePercentage"
            type="number"
            min="0"
            max="100"
            step="0.01"
            required
            addonAfter={<Button isStatic>%</Button>}
          />
        </Box>

        <Label required>
          <Trans zh="最低手续费" en="Minimum Handling Fee" />
        </Label>
        <Box>
          <HookFormInputField
            name="lowestFee"
            type="number"
            step="0.01"
            required
          />
        </Box>

        <Label required>
          <Trans zh="最高手续费" en="Maximum Handling Fee" />
        </Label>
        <Box required>
          <HookFormInputField
            name="highestFee"
            type="number"
            step="0.01"
            required
          />
        </Box>
      </FieldValues>
    </Stack>
  )
}

const ValueBlock = ({ children, ...props }) => (
  <Text.Block alignY="center" {...props}>
    {children}
  </Text.Block>
)
