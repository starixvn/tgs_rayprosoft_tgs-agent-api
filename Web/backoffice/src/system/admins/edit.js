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
import { Field as DepartmentSelectField } from "../departments/select"
import { Field as RoleSelectField } from "../groups/select"
import {
  Field as InputField,
  Password as PasswordField
} from "../../input/Field"
import { Select as StatusSelect, from as statusFrom } from "./status"
import { Create as CreateMenu, Update as UpdateMenu } from "./menu"
import { useIsDisplayMenuByRoute } from "../../user/context"

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
          white-space: nowrap;
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
          white-space: nowrap;
        `}
        {...dialog}
      >
        {editPassword()}
      </Modal.Disclosure>
      <PasswordDialog
        user={user}
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
  const deferFn = ([data]) =>
    client.put(`/api/users/${user.id}/password`, { data })
  const asyncSubmit = useAsync({ deferFn })

  return (
    <>
      <Formik
        initialValues={{
          password: "",
          confirmPassword: ""
        }}
        onSubmit={asyncSubmit.run}
      >
        <PasswordFormDialog dialog={dialog} />
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

const PasswordFormDialog = ({ dialog, finalFocusRef, ...props }) => {
  const { submitForm, resetForm, isPending } = useFormikContext()

  return (
    <Dialog.Modal
      title={passwordTitle()}
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
              "password passwordField"
              "confirmPassword confirmPasswordField";
          `}
        >
          <PasswordLabelInput />
          <ConfirmPasswordLabelInput />
        </Box>
      </Form>
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
      data
    })

  const deferFn = ([data]) => {
    data = {
      ...data,
      status: statusFrom(data.status)
    }
    return isUpdate ? update(data) : create(data)
  }

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
        status: statusFrom(user.status)
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
  const { submitForm, resetForm, isPending } = useFormikContext()

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
      title={isCreate ? createTitle() : updateTitle()}
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
          <UernameLabelInput
            user={user}
            isCreate={isCreate}
            isUpdate={isUpdate}
          />
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

const UernameLabelInput = ({ user, name = "username", isCreate, isUpdate }) => {
  return (
    <>
      <LabelBox name={name} label={username()} required={isCreate} />
      <FieldBox name={name}>
        {isCreate && <InputField name={name} required />}
        {isUpdate && <InputField name={name} value={user?.username} disabled />}
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
        <StatusSelect name={name} required />
      </FieldBox>
    </>
  )
}

//TODO: i18n
const createTitle = () => "新增管理员"

const updateTitle = () => "修改"

const passwordTitle = () => "修改密码"

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
