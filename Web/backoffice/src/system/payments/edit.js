import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/core"
import { Formik, Field, Form, useFormikContext } from "formik"
import {
  Stack,
  Box,
  Button,
  Text,
  Modal,
  Dialog,
  Columns,
  Heading,
  Flex,
  Group,
  TextareaField,
  List as _List,
  applyTheme,
  CheckboxField
} from "bumbag"
import { useModalState } from "../../dialog/Modal"
import { useIsDisplayMenuByRoute } from "../../user/context"
import { Update as UpdateMenu } from "./menu"
import { Trans, useTrans } from "../../i18n"
import { EditDisclosure } from "../../dialog/Disclosure"
import FormDialog, { ConfirmButtonFooter } from "../../form/Dialog"
import { FieldValues } from "../../layout/Grid"
import _Label from "../../form/Label"
import InputField from "../../input/Field"
import { ToggleButtonField } from "./status"
import { Type as TransferType } from "./transfer"
import { usePayment, useUpdatePayment, useUpload } from "./client"
import AlertDialog from "../../dialog/Alert"
import {
  RadioGroupField as ConditionRadioGroupField,
  AllPlayerText,
  AllPlayerSwitchField,
  AccumulatedDepositText,
  UnlimitedText,
  IdentityAuthenticationText,
  IdentityAuthenticationSwitchField,
  PhoneAuthenticationText,
  PhoneAuthenticationSwitchField,
  VipLevelsText
} from "./condition"
import { Field as VipLevelsCheckboxGroupField } from "../../vip-levels/CheckboxGroup"
import LoadingBackdrop from "../../dialog/LoadingBackdrop"
import { FieldGroup as NumberRangeInputFieldGroup } from "../../number/RangeInput"

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

const EditDialog = ({ record = {}, onSubmitted, dialog, ...props }) => {
  let title = useTrans({
    zh: "编辑",
    en: "Edit"
  })

  let {
    data = {},
    isLoading: isLoadingPayment,
    error: getPaymentDataError
  } = usePayment(record?.id)

  let [submitData, setSubmit] = useState()
  let { result, isLoading, error } = useUpdatePayment(record.id, submitData)

  if (isLoadingPayment) {
    return <LoadingBackdrop {...dialog} />
  }

  if (getPaymentDataError) {
    return <AlertDialog {...getPaymentDataError} onClickClose={dialog.hide} />
  }

  return (
    <>
      <FormDialog
        initialValues={{
          ...data.payment,
          reason: ""
        }}
        onSubmit={(data) => {
          setSubmit({
            ...record,
            ...data
          })
        }}
        result={result}
        isLoading={isLoading}
        error={error}
        onCloseErrorAlert={() => setSubmit(undefined)}
        onSubmitted={onSubmitted}
        footer={
          <ConfirmButtonFooter
            alignX="center"
            width="100%"
            isLoading={isLoading}
            disabled={isLoading}
          />
        }
        title={title}
        showCloseButton
        showActionButtons={false}
        maxWidth="unset"
        width="80vw"
        height="95vh"
        maxHeight="95vh"
        placement="top"
        display="flex"
        flexDirection="column"
        css={css`
          & .bb-Flex.bb-DialogContent {
            max-height: 95vh;
            flex: 1 auto;
            padding-bottom: 0.5em;
          }

          & .bb-DialogContent > .bb-Box {
            display: flex;
            flex-direction: column;
          }
        `}
        dialog={dialog}
      >
        <EditBody record={data.payment} />
      </FormDialog>
    </>
  )
}

const EditBody = ({ record = {}, dialog, ...props }) => {
  let { transferType } = record
  let { values, setFieldValue } = useFormikContext()
  let { transferLimitTo, transferLimitFrom } = values
  return (
    <Stack
      orientation="vertical"
      display="flex"
      flexDirection="column"
      flex="1 auto"
    >
      <Columns flex="1 auto" marginBottom="major-1">
        <Columns.Column>
          <Stack
            spacing="major-1"
            display="flex"
            flexDirection="column"
            height="100%"
          >
            <Heading use="h5">
              <Trans zh="基本资料" en="Basic" />
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
                <Trans zh="通道名称" en="Channel Name" />
              </Label>
              <InputField name="channelName" size="small" required />

              <Label>
                <Trans zh="状态" en="Status" />
              </Label>
              <Box>
                <ToggleButtonField name="paymentStatus" />
              </Box>

              <Label required>
                <Trans zh="单笔限额" en="Limit" />
              </Label>
              <NumberRangeInputFieldGroup
                names={["transferLimitFrom", "transferLimitTo"]}
                required
                min={[0, transferLimitFrom]}
                max={[transferLimitTo]}
                width="100%"
              />
            </FieldValues>

            <Columns flex="1 auto">
              <UploadListColumn
                title={<Trans zh="白名单列表" en="Whitelist" />}
                list={record.whitelist}
                onUploaded={(list) => setFieldValue("whitelist", list)}
                fileName="whitelist.xlsx"
              />
              <UploadListColumn
                title={<Trans zh="黑名单列表" en="Blacklist" />}
                list={record.blacklist}
                onUploaded={(list) => setFieldValue("blacklist", list)}
                fileName="blacklist.xlsx"
              />
            </Columns>
          </Stack>
        </Columns.Column>
        <Columns.Column>
          <Heading use="h5">
            <Trans zh="开放条件" en="Conditions" />
          </Heading>
          <FieldValues>
            <Label name="playerListCondition">
              <Trans zh="符合条件" en="Eligible" />
            </Label>
            <Box alignY="center">
              <ConditionRadioGroupField name="playerListCondition" />
            </Box>

            <Label name="allPlayer">
              <AllPlayerText />
            </Label>
            <Box alignY="center">
              <AllPlayerSwitchField width="auto" />
            </Box>

            <Label required>
              <AccumulatedDepositText />
            </Label>
            <Box alignY="center">
              <AccumulatedDepositGroup />
            </Box>

            <Label>
              <IdentityAuthenticationText />
            </Label>
            <Box alignY="center">
              <IdentityAuthenticationSwitchField />
            </Box>

            <Label>
              <PhoneAuthenticationText />
            </Label>
            <Box alignY="center">
              <PhoneAuthenticationSwitchField />
            </Box>

            <Label>
              <VipLevelsText />
            </Label>
            <Box>
              <VipLevelsCheckboxGroupField name="vipLevelIds" />
            </Box>
          </FieldValues>
        </Columns.Column>
      </Columns>
      <Stack spacing="major-1" orientation="horizontal">
        <Box flex="0 auto">
          <Trans zh="变更原因" en="Reason" />
        </Box>

        <Field component={TextareaField.Formik} name="reason" />
      </Stack>
    </Stack>
  )
}

const AccumulatedDepositGroup = () => {
  let { values } = useFormikContext()
  let { depositedAmountMin, depositedAmountMax } = values
  let [disabledDepositedAmountMax, setDisabledDepositedAmountMax] = useState(
    values?.depositedAmountUnlimited
  )

  return (
    <Stack orientation="horizontal" alignY="center">
      <NumberRangeInputFieldGroup
        names={["depositedAmountMin", "depositedAmountMax"]}
        required
        min={[0, depositedAmountMin]}
        max={[depositedAmountMax]}
        disabled={[false, disabledDepositedAmountMax]}
        width="100%"
      />
      <Field
        name="depositedAmountUnlimited"
        component={CheckboxField.Formik}
        onChange={(event) => {
          setDisabledDepositedAmountMax(event.target.checked)
        }}
        checkboxLabel={<UnlimitedText />}
        whiteSpace="nowrap"
      />
    </Stack>
  )
}

const Label = ({ fontWeight = "normal", children, ...props }) => (
  <_Label fontWeight={fontWeight} {...props}>
    {children}
  </_Label>
)

const ValueBlock = ({ children, ...props }) => (
  <Text.Block alignY="center" {...props}>
    {children}
  </Text.Block>
)

const List = applyTheme(_List, {
  styles: {
    base: {
      border: "default"
    }
  }
})

const UploadListColumn = ({ title, list = [], fileName = "", onUploaded }) => {
  let uploadFileInputRef = useRef()
  let [file, setFile] = useState()
  let { result, isLoading, error, clearCache } = useUpload(file)

  list = result?.players ?? list

  useEffect(() => {
    if (Array.isArray(result?.players) && !error) {
      onUploaded(result?.players)
    }
  }, [file, result])

  return (
    <Columns.Column display="flex" flexDirection="column">
      {error && (
        <AlertDialog {...error} onClickClose={() => setFile(undefined)} />
      )}
      <Stack orientation="horizontal" alignY="center" spacing="major-1">
        <Text.Block>{title}</Text.Block>
        <Button
          use="a"
          href="/download/payments/list.xlsx"
          download={fileName}
          palette="primary"
          size="small"
        >
          <Trans zh="格式" en="Format" />
        </Button>
      </Stack>
      <List overflowY="scroll" flex="1 0 5em" marginY="minor-1">
        {list.map((item) => (
          <List.Item key={item}>{item}</List.Item>
        ))}
      </List>
      <Box alignX="center">
        <input
          ref={uploadFileInputRef}
          type="file"
          onChange={(event) => {
            let [file] = event.target.files
            if (file) {
              let formData = new FormData()
              formData.append("file", file, file.name)
              setFile(formData)
            } else {
              clearCache()
              setFile(undefined)
            }
          }}
          css={css`
            display: none;
          `}
        />
        <Button
          palette="primary"
          type="button"
          onClick={(event) => {
            uploadFileInputRef?.current.click()
          }}
          size="small"
          isLoading={isLoading}
        >
          <Trans zh="上传" en="Upload" />
        </Button>
      </Box>
    </Columns.Column>
  )
}
