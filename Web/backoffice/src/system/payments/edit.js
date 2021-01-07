import React, { useState, useEffect, useMemo, useRef } from "react"
import { Stack, Box, Button, Text, Modal } from "bumbag"
import { useIsDisplayMenuByRoute } from "../../user/context"
import Label from "../../form/Label"
import Disclosure from "../../dialog/Disclosure"
import { useAddPaymentVendor, usePaymentVendorList } from "./client"
import { useAddPaymentChannel } from "../brandpayments/channels/client"
import { useAddPaymentMethod } from "../brandpayments/methods/client"
import { Trans } from "../../i18n"
import { FieldValues } from "../../layout/Grid"
import { InputField } from "../../input/Input"
import FormDialog from "../../form/FormDialog"

export const AddButton = ({ onSubmitted, ...props }) => {
  let dialog = Modal.useState()

  console.warn("TODO: vendor CreateMenu auth")
  /*
    let isDisplay = useIsDisplayMenuByRoute([CreateMenu])
    if (!isDisplay) {
      return null
    }
    */
  return (
    <>
      <Disclosure {...dialog}>
        <Trans zh="新增支付厂商" en="Add Payment Vendor" />
      </Disclosure>
      {dialog.visible && (
        <AddVendorDialog onSubmitted={onSubmitted} dialog={dialog} />
      )}
    </>
  )
}

const AddVendorDialog = ({ onSubmitted, dialog, ...props }) => {
  let [submitData, setSubmit] = useState()
  let { result, isLoading, error } = useAddPaymentVendor(submitData)

  const clearResult = () => {
    setSubmit(null)
  }

  //TODO: i18n
  let title = "Create Payment Vendor"
  return (
    <FormDialog
      defaultValues={{
        code: "",
        name: ""
      }}
      onSubmit={setSubmit}
      isLoading={isLoading}
      result={result}
      error={error}
      onCloseErrorAlert={clearResult}
      onSubmitted={onSubmitted}
      title={title}
      dialog={dialog}
    >
      <VendorEditBody />
    </FormDialog>
  )
}

const VendorEditBody = ({}) => {
  return (
    <FieldValues>
      <Label name="name" required>
        <Trans zh="支付厂商" en="Payment Vendor" />
      </Label>
      <Box>
        <InputField name="name" required />
      </Box>

      <Label name="code" required>
        <Trans zh="编码" en="Code" />
      </Label>
      <Box>
        <InputField name="code" required />
      </Box>
    </FieldValues>
  )
}

export const AddMethodButton = ({ paymentVendorId, onSubmitted, ...props }) => {
  let dialog = Modal.useState()

  console.warn("TODO: vendor CreateMenu auth")
  /*
      let isDisplay = useIsDisplayMenuByRoute([CreateMenu])
      if (!isDisplay) {
        return null
      }
      */

  console.warn("TODO: remove usePaymentVendorList")
  let { data = {} } = usePaymentVendorList({ showAll: true })
  let { items = [] } = data
  let [paymentVendor = {}] = items
  paymentVendorId = paymentVendor.id

  return (
    <>
      <Disclosure {...dialog}>
        <Trans zh="新增支付方式" en="Add Payment Method" />
      </Disclosure>
      {dialog.visible && (
        <AddPaymentMethodDialog
          paymentVendorId
          onSubmitted={onSubmitted}
          dialog={dialog}
        />
      )}
    </>
  )
}

const AddPaymentMethodDialog = ({
  paymentVendorId,
  onSubmitted,
  dialog,
  ...props
}) => {
  let [submitData, setSubmit] = useState()
  let { result, isLoading, error } = useAddPaymentMethod(submitData)

  const clearResult = () => {
    setSubmit(null)
  }

  //TODO: i18n
  let title = "Create Payment Method"
  return (
    <FormDialog
      defaultValues={{
        code: "",
        name: "",
        description: ""
      }}
      onSubmit={(data) => {
        setSubmit({
          paymentVendorId,
          ...data
        })
      }}
      isLoading={isLoading}
      result={result}
      error={error}
      onCloseErrorAlert={clearResult}
      onSubmitted={onSubmitted}
      title={title}
      dialog={dialog}
    >
      <PaymentMethodEditBody />
    </FormDialog>
  )
}

const PaymentMethodEditBody = ({}) => {
  return (
    <FieldValues>
      <Label name="name" required>
        <Trans zh="支付方式" en="Payment Name" />
      </Label>
      <Box>
        <InputField name="name" required />
      </Box>

      <Label name="code" required>
        <Trans zh="支付编码" en="Payment Code" />
      </Label>
      <Box>
        <InputField name="code" required />
      </Box>

      <Label name="description">
        <Trans zh="说明" en="Description" />
      </Label>
      <Box>
        <InputField name="description" />
      </Box>
    </FieldValues>
  )
}

export const AddChannelButton = ({ onSubmitted, ...props }) => {
  let dialog = Modal.useState()

  console.warn("TODO: vendor CreateMenu auth")
  /*
    let isDisplay = useIsDisplayMenuByRoute([CreateMenu])
    if (!isDisplay) {
        return null
    }
    */

  return (
    <>
      <Disclosure {...dialog}>
        <Trans zh="新增支付通道" en="Add Payment Channel" />
      </Disclosure>
      {dialog.visible && (
        <AddChannelDialog onSubmitted={onSubmitted} dialog={dialog} />
      )}
    </>
  )
}

const AddChannelDialog = ({ onSubmitted, dialog, ...props }) => {
  let [submitData, setSubmit] = useState()
  let { result, isLoading, error } = useAddPaymentChannel(submitData)

  const clearResult = () => {
    setSubmit(null)
  }

  //TODO: i18n
  let title = "Add Channel"
  return (
    <FormDialog
      defaultValues={{
        code: "",
        name: "",
        paymentVendorId: "",
        paymentMethodId: "",
        description: ""
      }}
      onSubmit={setSubmit}
      isLoading={isLoading}
      result={result}
      error={error}
      onCloseErrorAlert={clearResult}
      onSubmitted={onSubmitted}
      title={title}
      dialog={dialog}
    >
      <ChannelEditBOdy />
    </FormDialog>
  )
}

const ChannelEditBOdy = () => {
  return (
    <FieldValues>
      <Label name="name" required>
        <Trans zh="支付通道" en="Payment Channel" />
      </Label>
      <Box>
        <InputField name="name" />
      </Box>

      <Label name="code" required>
        <Trans zh="编码" en="Code" />
      </Label>
      <Box>
        <InputField name="code" required />
      </Box>

      <Label name="paymentVendorId" required>
        <Trans zh="支付厂商" en="Payment Vendor" />
      </Label>
      <Box>TODO: vendor select</Box>

      <Label name="paymentMethodId" required>
        <Trans zh="支付方式" en="Payment Method" />
      </Label>
      <Box>TODO: method select</Box>

      <Label name="description">
        <Trans zh="说明" en="Description" />
      </Label>
      <Box>
        <InputField name="description" required />
      </Box>
    </FieldValues>
  )
}
