import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/core"
import Async, {
  useAsync,
  IfPending,
  IfRejected,
  IfFulfilled
} from "react-async"
import client from "../../client"
import { Formik, Form, Field, useFormikContext } from "formik"
import {
  Spinner,
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
  Table,
  useClassName
} from "bumbag"
import LoadingBackdrop from "../../dialog/LoadingBackdrop"
import AlertDialog from "../../dialog/Alert"

export const CopyButton = ({ id, name, onSubmitted, ...props }) => {
  let dialog = Modal.useState()
  let disclosureRef = useRef()

  return (
    <>
      <Modal.Disclosure
        use={Button}
        ref={disclosureRef}
        css={css`
          flex: unset;
        `}
        {...dialog}
      >
        {copy()}
      </Modal.Disclosure>
      {dialog.visible && (
        <SubmitDialog
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

const copy = () => "复制"

const title = () => "复制权限组"

const inputLabel = () => "新权限组名称"
