import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/core"
import Async, { useAsync, IfRejected, IfFulfilled } from "react-async"
import { useLocation, useRouteMatch } from "react-router-dom"
import client from "../../client"
import { Formik, Field, Form, useFormikContext } from "formik"
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
  Checkbox,
  FieldStack,
  FieldWrapper,
  SelectField,
  Dialog,
  Modal,
  Table,
  useClassName
} from "bumbag"
import LoadingBackdrop from "../../dialog/LoadingBackdrop"
import AlertDialog from "../../dialog/Alert"
import { Create as CreateMenu, Update as UpdateMenu } from "./menu"
import { useIsDisplayMenuByRoute } from "../../user/context"
import { Field as InputField } from "../../input/Field"
import GridForm, { LabelBox, FieldBox } from "../../form/Grid"

export const AddButton = ({ onSubmitted, ...props }) => {
  let dialog = Modal.useState()
  let disclosureRef = useRef()

  let isDisplay = useIsDisplayMenuByRoute([CreateMenu])
  if (!isDisplay) {
    return null
  }

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
        {add()}
      </Modal.Disclosure>
      <AsyncDialog
        onSubmitted={onSubmitted}
        dialog={dialog}
        finalFocusRef={disclosureRef}
      />
    </>
  )
}

export const EditButton = ({ id = "", name = "", onSubmitted, ...props }) => {
  let dialog = Modal.useState()
  let disclosureRef = useRef()

  let isDisplay = useIsDisplayMenuByRoute([UpdateMenu])
  if (!isDisplay) {
    return null
  }

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
        {edit()}
      </Modal.Disclosure>
      <AsyncDialog
        id={id}
        name={name}
        onSubmitted={onSubmitted}
        dialog={dialog}
        finalFocusRef={disclosureRef}
      />
    </>
  )
}

const AsyncDialog = ({ id, name = "", dialog, onSubmitted, ...props }) => {
  let isUpdate = !!id
  let isCreate = !isUpdate

  const create = (data) =>
    client.post(`/api/department`, {
      data
    })

  const update = (data) =>
    client.put(`/api/department/${id}`, {
      data: {
        id,
        ...data
      }
    })

  const deferFn = ([data]) => {
    return id ? update(data) : create(data)
  }

  return (
    <Async deferFn={deferFn}>
      {(asyncSubmit) => {
        return (
          <>
            <Formik initialValues={{ name }} onSubmit={asyncSubmit.run}>
              <FormDialog
                dialog={dialog}
                isUpdate={isUpdate}
                isCreate={isCreate}
              ></FormDialog>
            </Formik>
            <Async.Rejected>
              {(error) => <AlertDialog {...error} />}
            </Async.Rejected>
            <Async.Fulfilled>
              {(_) => {
                useEffect(() => {
                  if (dialog.visible) {
                    dialog.hide()
                    onSubmitted && onSubmitted()
                  }
                }, [dialog.visible])

                return null
              }}
            </Async.Fulfilled>
          </>
        )
      }}
    </Async>
  )
}

const FormDialog = ({
  dialog,
  finalFocusRef,
  isCreate,
  isUpdate,
  ...props
}) => {
  const { submitForm, resetForm, isPending } = useFormikContext()
  return (
    <Dialog.Modal
      title={isCreate ? addTitle() : updateTitle()}
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
      <GridForm
        areas={`
        "name nameField"
        `}
      >
        <LabelBox name="name">{name()}</LabelBox>
        <FieldBox name="nameField">
          <InputField name="name" required />
        </FieldBox>
      </GridForm>
    </Dialog.Modal>
  )
}

const add = () => "新增"

const addTitle = () => "新增部门"

const updateTitle = () => "修改部门"

const name = () => "部门名称"

const edit = () => "修改"
