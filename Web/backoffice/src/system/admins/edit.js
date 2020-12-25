import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { Formik, Field, Form, useFormikContext } from "formik"
import { Stack, Box, Button, Text, Modal } from "bumbag"
import { Field as DepartmentSelectField } from "../departments/select"
import { Field as RoleSelectField } from "../roles/select"
import {
  Field as InputField,
  Password as PasswordField
} from "../../input/Field"
import { Trans, useTrans } from "../../i18n"
import { Select as StatusSelect } from "./status"
import { Create as CreateMenu, Update as UpdateMenu } from "./menu"
import { useIsDisplayMenuByRoute } from "../../user/context"
import { useAddAdmin, useEditAdmin, useEditPassword } from "./client"
import { FieldValues } from "../../layout/Grid"
import FormDialog from "../../form/Dialog"
import Label from "../../form/Label"
import Disclosure from "../../dialog/Disclosure"

export const AddButton = ({ onSubmitted, ...props }) => {
  let dialog = Modal.useState()

  let isDisplay = useIsDisplayMenuByRoute([CreateMenu])
  if (!isDisplay) {
    return null
  }

  return (
    <>
      <Disclosure {...dialog}>
        <Trans zh="新增" en="Add" />
      </Disclosure>
      {dialog.visible && (
        <AddDialog onSubmitted={onSubmitted} dialog={dialog} />
      )}
    </>
  )
}

export const EditButton = ({ user, onSubmitted, ...props }) => {
  let dialog = Modal.useState()

  let isDisplay = useIsDisplayMenuByRoute([UpdateMenu])
  if (!isDisplay) {
    return null
  }

  return (
    <>
      <Disclosure {...dialog}>
        <Trans zh="修改" en="Edit" />
      </Disclosure>
      {dialog.visible && (
        <EditDialog user={user} onSubmitted={onSubmitted} dialog={dialog} />
      )}
    </>
  )
}

export const EditPasswordButton = ({ user, onSubmitted, ...props }) => {
  let dialog = Modal.useState()

  let isDisplay = useIsDisplayMenuByRoute([UpdateMenu])
  if (!isDisplay) {
    return null
  }

  return (
    <>
      <Disclosure {...dialog}>
        <Trans zh="修改密码" en="Change Password" />
      </Disclosure>
      {dialog.visible && (
        <PasswordDialog user={user} onSubmitted={onSubmitted} dialog={dialog} />
      )}
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
  let [submitData, setSubmit] = useState()
  let { result, error, mutate, isLoading } = useEditPassword(
    user?.id,
    submitData
  )

  const clearResult = () => {
    setSubmit(null)
  }

  let title = usePasswordTitle()
  return (
    <FormDialog
      initialValues={{
        password: "",
        confirmPassword: ""
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
      <PasswordBody dialog={dialog} />
    </FormDialog>
  )
}

const PasswordBody = ({ dialog }) => {
  const { values, submitForm, resetForm } = useFormikContext()

  return (
    <Stack spacing="major-3" margin="major-1">
      <FieldValues>
        <Label name="password" required>
          <Trans zh="密码" en="Password" />
        </Label>
        <PasswordField name="password" autoComplete="new-password" />

        <Label name="confirmassword" required>
          <Trans zh="确认密码" en="Confirm Password" />
        </Label>
        <PasswordField
          name="confirmPassword"
          matchPassword={values?.password ?? ""}
          autoComplete="new-password"
        />
      </FieldValues>
    </Stack>
  )
}

const AddDialog = ({ onSubmitted, dialog, finalFocusRef, ...props }) => {
  let [submitData, setSubmit] = useState()
  let { result, error, mutate, isLoading } = useAddAdmin(submitData)

  const clearResult = () => {
    setSubmit(null)
  }

  let title = useCreateTitle()
  return (
    <FormDialog
      initialValues={{
        username: "",
        departmentId: "",
        roleId: "",
        name: "",
        password: "",
        confirmPassword: ""
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
      <AddBody dialog={dialog} />
    </FormDialog>
  )
}

const AddBody = ({ dialog }) => {
  const { values, submitForm, resetForm } = useFormikContext()

  return (
    <Stack spacing="major-3" margin="major-1">
      <FieldValues>
        <UsernameLabel required />
        <InputField name="username" required />

        <DepartmentLabel required />
        <Box>
          {dialog.visible && (
            <DepartmentSelectField name="departmentId" required />
          )}
        </Box>

        <RoleLabel required />
        <Box>
          {dialog.visible && <RoleSelectField name="roleId" required />}
        </Box>

        <NameLabel required />
        <InputField name="name" required />

        <Label name="password" required>
          <Trans zh="密码" en="Password" />
        </Label>
        <PasswordField name="password" autoComplete="new-password" />

        <Label name="confirmPassword" required>
          <Trans zh="确认密码" en="Confirm Password" />
        </Label>
        <PasswordField
          name="confirmPassword"
          matchPassword={values?.password ?? ""}
          autoComplete="new-password"
        />
      </FieldValues>
    </Stack>
  )
}

const UsernameLabel = ({ name = "username", required }) => (
  <Label name={name} required={required}>
    <Trans zh="用户名" en="Username" />
  </Label>
)

const DepartmentLabel = ({ name = "departmentId", required }) => (
  <Label name={name} required={required}>
    <Trans zh="部门" en="Department" />
  </Label>
)

const RoleLabel = ({ name = "roleId", required }) => (
  <Label name={name} required={required}>
    <Trans zh="权限组" en="Role" />
  </Label>
)

const NameLabel = ({ name = "name", required }) => (
  <Label name={name} required={required}>
    <Trans zh="姓名" en="Name" />
  </Label>
)

const EditDialog = ({
  user = {},
  onSubmitted,
  dialog,
  finalFocusRef,
  ...props
}) => {
  let [submitData, setSubmit] = useState()
  let { result, error, mutate, isLoading } = useEditAdmin(user?.id, submitData)

  const submit = (data) => {
    setSubmit(data)
  }

  const clearResult = () => {
    setSubmit(null)
  }

  let title = useUpdateTitle()
  return (
    <FormDialog
      initialValues={{
        departmentId: user.departmentId,
        roleId: user.roleId,
        name: user.name,
        status: user.status
      }}
      onSubmit={submit}
      isLoading={isLoading}
      result={result}
      error={error}
      onCloseErrorAlert={clearResult}
      onSubmitted={onSubmitted}
      title={title}
      dialog={dialog}
      finalFocusRef={finalFocusRef}
    >
      <EditBody user={user} dialog={dialog} />
    </FormDialog>
  )
}

const EditBody = ({ user, dialog }) => {
  const { values, submitForm, resetForm } = useFormikContext()
  return (
    <Stack spacing="major-3" margin="major-1">
      <FieldValues>
        <UsernameLabel />
        <InputField name="username" value={user?.username} disabled />

        <DepartmentLabel />
        <Box>
          {dialog.visible && (
            <DepartmentSelectField name="departmentId" required />
          )}
        </Box>

        <RoleLabel />
        <Box>
          {dialog.visible && <RoleSelectField name="roleId" required />}
        </Box>

        <NameLabel />
        <InputField name="name" required />

        <Label htmlFor="status">
          <Trans zh="状态" en="Status" />
          <Text color="danger">*</Text>
        </Label>
        <StatusSelect name="status" required />
      </FieldValues>
    </Stack>
  )
}

const useCreateTitle = () =>
  useTrans({
    zh: "新增管理员",
    en: "Add Admin"
  })

const useUpdateTitle = () =>
  useTrans({
    zh: "修改",
    en: "Edit"
  })

const usePasswordTitle = () =>
  useTrans({
    zh: "修改密码",
    en: "Change Password"
  })
