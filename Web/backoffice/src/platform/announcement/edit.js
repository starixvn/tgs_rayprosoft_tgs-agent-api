import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/core"
import { Formik, Field, Form, useFormikContext } from "formik"
import {
  Stack,
  Box,
  Button,
  Text,
  Modal,
  Textarea,
  TextareaField,
  styled,
  border,
  palette
} from "bumbag"
import {
  Create as CreateMenu,
  Update as UpdateMenu,
  Verify as VerifyMenu,
  View as ViewMenu
} from "./menu"
import { useIsDisplayMenuByRoute } from "../../user/context"
import { FieldValues } from "../../layout/Grid"
import FormDialog from "../../form/Dialog"
import Label from "../../form/Label"
import Disclosure from "../../dialog/Disclosure"
import {
  useAddAnnouncement,
  useEditAnnouncement,
  useVerifyAnnouncement
} from "./client"
import { Trans, useTrans } from "../../i18n"
import { Field as InputField } from "../../input/Field"
import DateField from "../../date/Field"
import ModalDialog from "../../dialog/Modal"
import parseDate from "date-fns/fp/parseISO"
import { CancelText } from "../../button/ActionButtons"

export const AddButton = ({ onSubmitted, ...props }) => {
  let dialog = Modal.useState()

  let isDisplay = useIsDisplayMenuByRoute([CreateMenu])
  if (!isDisplay) {
    return null
  }

  return (
    <>
      <Disclosure {...dialog} {...props}>
        <Trans zh="新增" en="Add" />
      </Disclosure>
      <AddDialog onSubmitted={onSubmitted} dialog={dialog} />
    </>
  )
}

const AddDialog = ({ onSubmitted, dialog, finalFocusRef, ...props }) => {
  let [submitData, setSubmit] = useState()
  let { result, error, mutate, isLoading } = useAddAnnouncement({
    data: submitData
  })

  const clearResult = () => {
    setSubmit(null)
  }

  let title = useTrans({
    zh: "新增公告",
    en: "Add Announcement"
  })

  return (
    <FormDialog
      initialValues={{
        subject: "",
        from: null,
        to: null,
        content: ""
      }}
      onSubmit={setSubmit}
      isLoading={isLoading}
      result={result}
      error={error}
      onCloseErrorAlert={clearResult}
      onSubmitted={onSubmitted}
      title={title}
      dialog={dialog}
      finalFocusRef={finalFocusRef}
      placement="top"
      css={DialogCss}
    >
      <EditBody />
    </FormDialog>
  )
}

export const EditButton = ({ record, onSubmitted, ...props }) => {
  let dialog = Modal.useState()

  let isDisplay = useIsDisplayMenuByRoute([UpdateMenu])
  if (!isDisplay) {
    return null
  }

  return (
    <>
      <Disclosure {...dialog} {...props}>
        <Trans zh="修改" en="Edit" />
      </Disclosure>
      <EditDialog record={record} onSubmitted={onSubmitted} dialog={dialog} />
    </>
  )
}

export const ViewButton = ({ record, ...props }) => {
  let dialog = Modal.useState()

  let isDisplay = useIsDisplayMenuByRoute([ViewMenu])
  if (!isDisplay) {
    return null
  }

  return (
    <>
      <Disclosure {...dialog} {...props}>
        <Trans zh="查看" en="View" />
      </Disclosure>
      <ViewDialog record={record} dialog={dialog} />
    </>
  )
}

const ViewDialog = ({ record, dialog, finalFocusRef, ...props }) => {
  let from = parseDate(record.from)
  let to = parseDate(record.to)

  let title = useTrans({
    zh: "查看公告",
    en: "View Announcement"
  })
  return (
    <FormDialog
      initialValues={{
        ...record,
        from,
        to
      }}
      title={title}
      placement="top"
      css={DialogCss}
      dialog={dialog}
      showActionButtons={false}
      footer={
        <Box alignX="right" width="100%">
          <Button onClick={dialog.hide}>
            <CancelText />
          </Button>
        </Box>
      }
    >
      <VerifyBody readonly />
    </FormDialog>
  )
}

const EditDialog = ({
  record,
  onSubmitted,
  dialog,
  finalFocusRef,
  ...props
}) => {
  let { id } = record
  let [submitData, setSubmit] = useState()
  let { result, error, mutate, isLoading } = useEditAnnouncement({
    id,
    data: submitData
  })

  const clearResult = () => {
    setSubmit(null)
  }

  let title = useTrans({
    zh: "修改公告",
    en: "Edit Announcement"
  })

  let from = parseDate(record.from)
  let to = parseDate(record.to)
  return (
    <FormDialog
      initialValues={{
        ...record,
        from,
        to
      }}
      onSubmit={setSubmit}
      isLoading={isLoading}
      result={result}
      error={error}
      onCloseErrorAlert={clearResult}
      onSubmitted={onSubmitted}
      title={title}
      dialog={dialog}
      finalFocusRef={finalFocusRef}
      placement="top"
      css={DialogCss}
    >
      <EditBody />
    </FormDialog>
  )
}

const EditBody = ({ readonly = false }) => {
  let formContext = useFormikContext()
  let minDate = readonly ? null : Reflect.get(formContext?.values, "from")
  let maxDate = readonly ? null : Reflect.get(formContext?.values, "to")

  let required = !readonly
  let textareaHeight = "15vh"
  return (
    <FieldValues>
      <Label name="subject" required={required}>
        <Trans zh="主题" en="Subject" />
      </Label>
      <InputField name="subject" required={required} disabled={readonly} />

      <Label name="from" required={required}>
        <Trans zh="开始时间" en="Start" />
      </Label>
      <Box>
        <DateField
          name="from"
          maxDate={maxDate}
          required={required}
          disabled={readonly}
        />
      </Box>

      <Label name="to" required={required}>
        <Trans zh="结束时间" en="End" />
      </Label>
      <Box>
        <DateField
          name="to"
          minDate={minDate}
          required={required}
          disabled={readonly}
        />
      </Box>

      <Label name="content" required={required}>
        <Trans zh="内容" en="Content" />
      </Label>
      <Field
        component={TextareaField.Formik}
        name="content"
        required={required}
        disabled={readonly}
        height={textareaHeight}
        css={css`
          height: ${textareaHeight};

          & .bb-FieldWrapper,
          & .bb-TextareaWrapper,
          & .bb-Textarea {
            height: ${textareaHeight};
          }
        `}
      />
    </FieldValues>
  )
}

export const VerifyButton = ({ record, onSubmitted, ...props }) => {
  let dialog = Modal.useState()

  let isDisplay = useIsDisplayMenuByRoute([VerifyMenu])
  if (!isDisplay) {
    return null
  }

  return (
    <>
      <Disclosure {...dialog} {...props}>
        <Trans zh="审核" en="Verify" />
      </Disclosure>
      <VerifyDialog record={record} onSubmitted={onSubmitted} dialog={dialog} />
    </>
  )
}

const DialogCss = css`
  min-width: 65vw;
  min-height: 65vh;

  & .bb-DialogContent {
    min-height: 65vh;
  }
`

const VerifyDialog = ({
  record,
  onSubmitted,
  dialog,
  finalFocusRef,
  ...props
}) => {
  let [type, setType] = useState()
  let [submitData, setSubmit] = useState()
  let { result, error, mutate, isLoading } = useVerifyAnnouncement({
    id: record?.id,
    type,
    data: submitData
  })

  const clearResult = () => {
    setSubmit(null)
  }

  let title = useTrans({
    zh: "审核公告",
    en: "Verify Announcement"
  })
  let from = parseDate(record.from)
  let to = parseDate(record.to)
  return (
    <FormDialog
      initialValues={{
        ...record,
        from,
        to,
        remarks: ""
      }}
      onSubmit={setSubmit}
      clickSubmit={({ submitForm }) => (event) => {
        setType("pass")
        submitForm()
      }}
      addonButtons={({ submitForm }) => {
        return (
          <Button
            onClick={() => {
              setType("reject")
              submitForm()
            }}
            palette="danger"
            variant="outlined"
          >
            <Trans zh="拒绝" en="Reject" />
          </Button>
        )
      }}
      isLoading={isLoading}
      result={result}
      error={error}
      onCloseErrorAlert={clearResult}
      onSubmitted={onSubmitted}
      title={title}
      placement="top"
      css={DialogCss}
      dialog={dialog}
    >
      <VerifyBody />
    </FormDialog>
  )
}

const VerifyBody = ({ readonly = false }) => {
  return (
    <Stack spacing="major-2">
      <EditBody readonly={true} />
      <Box border="default" height="1px" width="100%"></Box>
      <FieldValues>
        <Label name="remarks">
          <Trans zh="备注" en="Remarks" />
        </Label>
        <Field
          name="remarks"
          component={TextareaField.Formik}
          disabled={readonly}
        />
      </FieldValues>
    </Stack>
  )
}
