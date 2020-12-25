import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { Formik, Form, Field, useFormikContext } from "formik"
import {
  Stack,
  Flex,
  Box,
  Card,
  Set,
  Button,
  Text,
  Label,
  Input,
  InputField,
  Checkbox,
  FieldStack,
  SelectField,
  Dialog,
  Modal,
  useClassName
} from "bumbag"
import LoadingBackdrop from "../../dialog/LoadingBackdrop"
import AlertDialog from "../../dialog/Alert"
import { Copy as CopyMenu } from "./menu"
import { useIsDisplayMenuByRoute } from "../../user/context"
import Disclosure from "../../dialog/Disclosure"
import FormDialog from "../../form/Dialog"
import { useCopyRole } from "./client"
import { Trans, useTrans } from "../../i18n"

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

const CopyDialog = ({
  id,
  name = "",
  onSubmitted,
  dialog,
  finalFocusRef,
  ...props
}) => {
  let [submitData, setSubmit] = useState()
  let { result, error, mutate, isLoading } = useCopyRole(id, submitData)

  const clearResult = () => {
    setSubmit(null)
    //mutate()
  }

  let title = useTrans({
    zh: "复制权限组",
    en: "Copy Role"
  })
  let inputRef = useRef()
  return (
    <FormDialog
      initialValues={{
        name
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
    >
      <Field
        name="name"
        isRequired
        component={InputField.Formik}
        label={<Trans zh="新权限组名称" en="New role name" />}
        inputRef={inputRef}
      />
    </FormDialog>
  )
}

/*
const SubmitDialog = ({
  id,
  name = "",
  onSubmitted,
  dialog,
  finalFocusRef,
  ...props
}) => {
  const deferFn = ([data]) => {
    return client.post(`/api/role/${id}/copy`, { data })
  }

  let inputRef = useRef()

  return (
    <Async deferFn={deferFn}>
      {(submit) => {
        return (
          <>
            <Formik
              initialValues={{ name: "" }}
              onSubmit={(form) => {
                let data = {
                  ...form,
                  id
                }

                submit.run(data)
              }}
            >
              <CopyFormDialog
                dialog={dialog}
                inputRef={inputRef}
                finalFocusRef={finalFocusRef}
              />
            </Formik>
            <Async.Rejected>
              {(error) => <AlertDialog {...error} finalFocusRef={inputRef} />}
            </Async.Rejected>
            <Async.Fulfilled>
              {(_) => {
                useEffect(() => {
                  dialog.hide()
                  onSubmitted && onSubmitted()
                })

                return null
              }}
            </Async.Fulfilled>
          </>
        )
      }}
    </Async>
  )
}

const CopyFormDialog = ({ dialog, inputRef, finalFocusRef, ...props }) => {
  const { values, submitForm, resetForm, isPending } = useFormikContext()

  useEffect(() => {
    if (dialog.visible) {
      inputRef?.current?.focus()
    }
  }, [dialog.visible])

  return (
    <Dialog.Modal
      title={title()}
      showActionButtons
      actionButtonsProps={{
        isLoading: isPending,
        onClickSubmit: submitForm,
        onClickCancel: resetForm
      }}
      hideOnClickOutside={false}
      unstable_finalFocusRef={finalFocusRef}
      {...dialog}
    >
      <Form>
        <Field
          name="name"
          isRequired
          component={InputField.Formik}
          label={inputLabel()}
          inputRef={inputRef}
        />
      </Form>
    </Dialog.Modal>
  )
}
*/
