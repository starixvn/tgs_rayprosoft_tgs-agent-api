import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/core"
import Async, { useAsync, IfRejected, IfFulfilled } from "react-async"
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
import {
  label as systemSubMenuLabel,
  Code as SystemCode,
  name as systemModuleLabel
} from "../index"
import { label as menuLabel, find as findMenu } from "../../menu"
import LoadingBackdrop from "../../dialog/LoadingBackdrop"
import AlertDialog from "../../dialog/Alert"
import { Field as DepartmentSelectField } from "../departments/select"
import { Field as RoleSelectField } from "../groups/select"
import InputField from "../../input/Field"

export const AddButton = ({ onSubmitted, ...props }) => {
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
        {add()}
      </Modal.Disclosure>
      {dialog.visible && (
        <SubmitDialog
          onSubmitted={onSubmitted}
          dialog={dialog}
          finalFocusRef={disclosureRef}
        />
      )}
    </>
  )
}

const SubmitDialog = ({
  user = {},
  onSubmitted,
  dialog,
  finalFocusRef,
  ...props
}) => {
  let { id } = user

  const create = (data) =>
    client.post(`/api/users`, {
      data
    })

  const update = (data) =>
    client.put(`/api/users/${id}`, {
      data: {
        ...role,
        ...data
      }
    })

  const deferFn = ([data]) => (id ? update(data) : create(data))

  let sumit = useAsync({ deferFn })
  return (
    <>
      <Formik
        initialValues={{
          username: "",
          departmentName: "",
          role: "",
          password: ""
        }}
        onSubmit={(data) => {
          submit.run(data)
        }}
      >
        <FormDialog user={user} dialog={dialog}></FormDialog>
      </Formik>
      <IfRejected state={sumit}>
        {(error) => <AlertDialog {...error} finalFocusRef={inputRef} />}
      </IfRejected>
      <IfFulfilled state={sumit}>
        {(_) => {
          useEffect(() => {
            dialog.hide()
            onSubmitted && onSubmitted()
          })

          return null
        }}
      </IfFulfilled>
    </>
  )
}

const FormDialog = ({
  user = {},
  dialog,
  inputRef,
  finalFocusRef,
  ...props
}) => {
  const { values, submitForm, resetForm, isPending } = useFormikContext()

  let { id } = user
  let isUpdate = !!id
  let isCreate = !isUpdate
  /*
  useEffect(() => {
    if (dialog.visible) {
      inputRef?.current?.focus()
    }
  }, [dialog.visible])
  */

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
        <Box
          css={css`
            display: grid;
            grid-template-rows: auto;
            grid-template-areas:
              "username usernameField"
              "department departmentField"
              "role roleField"
              "name nameField"
              "status statusField"
              "password passwordField"
              "confirmPassword confirmPasswordField";
          `}
        >
          <UernameLabelInput />
          <DepartmentLabelSelect />
          <RoleLabelSelect />
          <NameLabelInput />
          {isCreate && <PasswordLabelInput />}
          {isCreate && <ConfirmPasswordLabelInput />}
          {isUpdate && <StatusLabelSelect />}
        </Box>
      </Form>
    </Dialog.Modal>
  )
}

const LabelBox = ({ name = "", label = "", required = true }) => {
  return (
    <Box
      css={css`
        grid-area: ${name};
      `}
      margin="major-1"
    >
      <Label
        htmlFor={name}
        marginTop="major-1"
        css={css`
          line-height: unset;
        `}
      >
        {label}
        {required && <Text color="danger">*</Text>}
      </Label>
    </Box>
  )
}

const FieldBox = ({ name, children, ...props }) => {
  return (
    <Box
      css={css`
        grid-area: ${name}Field;
      `}
      margin="major-1"
    >
      {children}
    </Box>
  )
}

const UernameLabelInput = ({ name = "username" }) => {
  return (
    <>
      <LabelBox name={name} label={username()} />
      <FieldBox name={name}>
        <InputField name={name} required />
      </FieldBox>
    </>
  )
}

const DepartmentLabelSelect = ({ name = "department" }) => {
  return (
    <>
      <LabelBox name={name} label={departmentName()} />
      <FieldBox name={name}>
        <DepartmentSelectField name={name} required />
      </FieldBox>
    </>
  )
}

const RoleLabelSelect = ({ name = "role" }) => {
  return (
    <>
      <LabelBox name={name} label={role()} />
      <FieldBox name={name}>
        <RoleSelectField name={name} required />
      </FieldBox>
    </>
  )
}

const NameLabelInput = ({ name = "name" }) => {
  return (
    <>
      <LabelBox name={name} label={firstNameLastName()} />
      <FieldBox name={name}>
        <InputField name={name} required />
      </FieldBox>
    </>
  )
}

//TODO: password validation
const PasswordLabelInput = ({ name = "password" }) => {
  return (
    <>
      <LabelBox name={name} label={password()} />
      <FieldBox name={name}>
        <InputField name={name} type="password" />
      </FieldBox>
    </>
  )
}

//TODO: password validation
const ConfirmPasswordLabelInput = ({ name = "confirmPassword" }) => {
  return (
    <>
      <LabelBox name={name} label={confirmPassword()} />
      <FieldBox name={name}>
        <InputField name={name} type="password" />
      </FieldBox>
    </>
  )
}

const StatusLabelSelect = ({ name = "status" }) => {
  return (
    <>
      <LabelBox name={name} label={status()} />
      <FieldBox name={name}>
        <InputField name={name} type="password" />
      </FieldBox>
    </>
  )
}

//TODO: i18n
const title = () => "新增管理员"

const add = () => "新增"

const username = () => "用户名"

const departmentName = () => "部门"

const role = () => "权限组"

const firstNameLastName = () => "姓名"

const password = () => "密码"

const confirmPassword = () => "确认密码"

const status = () => "状态"
