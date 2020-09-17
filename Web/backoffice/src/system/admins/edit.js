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
import {
  Field as InputField,
  Password as PasswordField
} from "../../input/Field"

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

export const EditButton = ({ user, onSubmitted, ...props }) => {
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
        {edit()}
      </Modal.Disclosure>
      {dialog.visible && (
        <SubmitDialog
          user={user}
          onSubmitted={onSubmitted}
          dialog={dialog}
          finalFocusRef={disclosureRef}
        />
      )}
    </>
  )
}

export const EditPasswordButton = ({ user, onSubmitted, ...props }) => {
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
        {editPassword()}
      </Modal.Disclosure>
      <PasswordDialog
        user={user}
        editPassword
        onSubmitted={onSubmitted}
        dialog={dialog}
        finalFocusRef={disclosureRef}
      />
    </>
  )
}

const PasswordDialog = ({
  user = {},
  onSubmitted,
  dialog,
  finalFocusRef,
  ...props
}) => {
  return (
    <Dialog.Modal
      title={title()}
      unstable_finalFocusRef={finalFocusRef}
      {...dialog}
    >
      <Input />
    </Dialog.Modal>
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
  let isUpdate = !!id
  let isCreate = !isUpdate

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

  const deferFn = ([data]) => (isUpdate ? update(data) : create(data))

  let initialValues = isCreate
    ? {
        username: "",
        departmentId: "",
        roleId: "",
        name: "",
        password: "",
        confirmPassword: ""
      }
    : {
        departmentId: user.departmentId,
        roleId: user.roleId,
        name: user.name,
        status: user.status
      }
  let asyncSubmit = useAsync({ deferFn })
  return (
    <>
      <Formik initialValues={initialValues} onSubmit={asyncSubmit.run}>
        <FormDialog user={user} dialog={dialog}></FormDialog>
      </Formik>
      <IfRejected state={asyncSubmit}>
        {(error) => <AlertDialog {...error} />}
      </IfRejected>
      <IfFulfilled state={asyncSubmit}>
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
  const {
    values,
    errors,
    submitForm,
    resetForm,
    isPending
  } = useFormikContext()

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
        submitText: confirmLabel(),
        cancelText: cancelLabel(),
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
              "departmentId departmentIdField"
              "roleId roleIdField"
              "name nameField"
              "password passwordField"
              "confirmPassword confirmPasswordField"
              "status statusField";
          `}
        >
          <UernameLabelInput disabled={isUpdate} />
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
      marginBottom="0"
    >
      {children}
    </Box>
  )
}

const UernameLabelInput = ({ name = "username", disabled }) => {
  return (
    <>
      <LabelBox name={name} label={username()} />
      <FieldBox name={name}>
        <InputField name={name} required={!disabled} disabled={disabled} />
      </FieldBox>
    </>
  )
}

const DepartmentLabelSelect = ({ name = "departmentId" }) => {
  return (
    <>
      <LabelBox name={name} label={departmentName()} />
      <FieldBox name={name}>
        <DepartmentSelectField name={name} required />
      </FieldBox>
    </>
  )
}

const RoleLabelSelect = ({ name = "roleId" }) => {
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

const PasswordLabelInput = ({ name = "password" }) => {
  return (
    <>
      <LabelBox name={name} label={password()} />
      <FieldBox name={name}>
        <PasswordField name={name} autoComplete="new-password" />
      </FieldBox>
    </>
  )
}

const ConfirmPasswordLabelInput = ({ name = "confirmPassword" }) => {
  const { values = {} } = useFormikContext()
  return (
    <>
      <LabelBox name={name} label={confirmPassword()} />
      <FieldBox name={name}>
        <PasswordField
          name={name}
          matchPassword={values?.password ?? ""}
          autoComplete="new-password"
        />
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

const edit = () => "修改"

const editPassword = () => "修改密码"

const username = () => "用户名"

const departmentName = () => "部门"

const role = () => "权限组"

const firstNameLastName = () => "姓名"

const password = () => "密码"

const confirmPassword = () => "确认密码"

const status = () => "状态"

const confirmLabel = () => "确定"

const cancelLabel = () => "取消"
