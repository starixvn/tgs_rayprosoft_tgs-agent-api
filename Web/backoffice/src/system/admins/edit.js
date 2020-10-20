import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/core"
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
import { Field as RoleSelectField } from "../roles/select"
import {
  Field as InputField,
  Password as PasswordField
} from "../../input/Field"
import { Trans, useTrans } from "../../i18n"
import { Select as StatusSelect, from as statusFrom } from "./status"
import { Create as CreateMenu, Update as UpdateMenu } from "./menu"
import { useIsDisplayMenuByRoute } from "../../user/context"
import { useAddAdmin, useEditAdmin, useEditPassword } from "./client"
import { ConfirmCancel } from "../../button/ActionButtons"
import { FieldValues } from "../../layout/Grid"

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
        <Trans zh="新增" en="Add" />
      </Modal.Disclosure>
      {dialog.visible && (
        <AddDialog
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
        <Trans zh="修改" en="Edit" />
      </Modal.Disclosure>
      {dialog.visible && (
        <EditDialog
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
        <Trans zh="修改密码" en="Change Password" />
      </Modal.Disclosure>
      {dialog.visible && (
        <PasswordDialog
          user={user}
          onSubmitted={onSubmitted}
          dialog={dialog}
          finalFocusRef={disclosureRef}
        />
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
  let { result, error, mutate, isLoading } = useEditPassword({
    id: user?.id,
    data: submitData
  })
  useEffect(() => {
    if (result && !error) {
      onSubmitted && onSubmitted()
      dialog.hide()
    }
  }, [result])

  const clearResult = () => {
    setSubmit(null)
  }

  return (
    <Dialog.Modal
      title={usePasswordTitle()}
      hideOnClickOutside={false}
      unstable_finalFocusRef={finalFocusRef}
      {...dialog}
    >
      {error && <AlertDialog {...error} onClickClose={clearResult} />}
      <Formik
        initialValues={{
          password: "",
          confirmPassword: ""
        }}
        onSubmit={setSubmit}
      >
        <PasswordFormDialog isLoading={isLoading} dialog={dialog} />
      </Formik>
    </Dialog.Modal>
  )
}

const PasswordFormDialog = ({ isLoading, dialog, finalFocusRef, ...props }) => {
  const { values, submitForm, resetForm } = useFormikContext()

  return (
    <Form>
      <Stack spacing="major-3" margin="major-1">
        <FieldValues>
          <Label
            htmlFor="password"
            css={css`
              line-height: unset;
            `}
          >
            <Trans zh="密码" en="Password" />
            <Text color="danger">*</Text>
          </Label>
          <PasswordField
            autoFocus={true}
            name="password"
            autoComplete="new-password"
          />

          <Label
            htmlFor="confirmassword"
            css={css`
              line-height: unset;
            `}
          >
            <Trans zh="确认密码" en="Confirm Password" />
            <Text color="danger">*</Text>
          </Label>
          <PasswordField
            name="confirmPassword"
            matchPassword={values?.password ?? ""}
            autoComplete="new-password"
          />
        </FieldValues>
        <Box alignX="right">
          <ConfirmCancel
            onClickSubmit={submitForm}
            onClickCancel={dialog.hide}
          />
        </Box>
      </Stack>
    </Form>
  )
}

const AddDialog = ({
  user = {},
  onSubmitted,
  dialog,
  finalFocusRef,
  ...props
}) => {
  let [submitData, setSubmit] = useState()
  let { result, error, mutate, isLoading } = useAddAdmin({
    data: submitData
  })
  useEffect(() => {
    if (result && !error) {
      onSubmitted && onSubmitted()
      dialog.hide()
    }
  }, [result])

  const clearResult = () => {
    setSubmit(null)
  }

  let title = useCreateTitle()
  return (
    <Dialog.Modal
      title={title}
      hideOnClickOutside={false}
      unstable_finalFocusRef={finalFocusRef}
      {...dialog}
    >
      {error && <AlertDialog {...error} onClickClose={clearResult} />}
      <Formik
        initialValues={{
          username: "",
          departmentId: "",
          roleId: "",
          name: "",
          password: "",
          confirmPassword: ""
        }}
        onSubmit={setSubmit}
      >
        <AddForm
          isLoading={isLoading}
          dialog={dialog}
          onClickCancel={dialog.hide}
        />
      </Formik>
    </Dialog.Modal>
  )
}

const AddForm = ({ onClickCancel }) => {
  const { values, submitForm, resetForm } = useFormikContext()

  return (
    <Form>
      <Stack spacing="major-3" margin="major-1">
        <FieldValues>
          <UsernameLabel required />
          <InputField name="username" autoFocus required />

          <DepartmentLabel />
          <DepartmentSelectField name="departmentId" required />

          <RoleLabel />
          <RoleSelectField name="roleId" required />

          <NameLabel />
          <InputField name="name" required />

          <Label htmlFor="password">
            <Trans zh="密码" en="Password" />
            <Text color="danger">*</Text>
          </Label>
          <PasswordField name="password" autoComplete="new-password" />

          <Label htmlFor="confirmPassword">
            <Trans zh="确认密码" en="Confirm Password" />
            <Text color="danger">*</Text>
          </Label>
          <PasswordField
            name="confirmPassword"
            matchPassword={values?.password ?? ""}
            autoComplete="new-password"
          />
        </FieldValues>
        <Box alignX="right">
          <ConfirmCancel
            onClickSubmit={submitForm}
            onClickCancel={onClickCancel}
          />
        </Box>
      </Stack>
    </Form>
  )
}

const UsernameLabel = ({ name = "username", required }) => (
  <Label htmlFor={name}>
    <Trans zh="用户名" en="Username" />
    {required && <Text color="danger">*</Text>}
  </Label>
)

const DepartmentLabel = ({ name = "departmentId" }) => (
  <Label htmlFor={name}>
    <Trans zh="部门" en="Department" />
    <Text color="danger">*</Text>
  </Label>
)

const RoleLabel = ({ name = "roleId" }) => (
  <Label htmlFor={name}>
    <Trans zh="权限组" en="Role" />
    <Text color="danger">*</Text>
  </Label>
)

const NameLabel = ({ name = "name" }) => (
  <Label htmlFor={name}>
    <Trans zh="姓名" en="Name" />
    <Text color="danger">*</Text>
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
  let { result, error, mutate, isLoading } = useEditAdmin({
    id: user?.id,
    data: submitData
  })
  useEffect(() => {
    if (result && !error) {
      onSubmitted && onSubmitted()
      dialog.hide()
    }
  }, [result])

  const submit = (data) => {
    data = {
      ...data,
      status: statusFrom(data.status)
    }
    setSubmit(data)
  }

  const clearResult = () => {
    setSubmit(null)
  }

  let title = useUpdateTitle()
  return (
    <Dialog.Modal
      title={title}
      hideOnClickOutside={false}
      unstable_finalFocusRef={finalFocusRef}
      {...dialog}
    >
      {error && <AlertDialog {...error} onClickClose={clearResult} />}
      <Formik
        initialValues={{
          departmentId: user.departmentId,
          roleId: user.roleId,
          name: user.name,
          status: user.status
        }}
        onSubmit={submit}
      >
        <EditForm
          user={user}
          isLoading={isLoading}
          dialog={dialog}
          onClickCancel={dialog.hide}
        />
      </Formik>
    </Dialog.Modal>
  )
}

const EditForm = ({ user, onClickCancel }) => {
  const { values, submitForm, resetForm } = useFormikContext()

  return (
    <Form>
      <Stack spacing="major-3" margin="major-1">
        <FieldValues>
          <UsernameLabel />
          <InputField name="username" value={user?.username} disabled />

          <DepartmentLabel />
          <DepartmentSelectField name="departmentId" required />

          <RoleLabel />
          <RoleSelectField name="roleId" required />

          <NameLabel />
          <InputField name="name" required />

          <Label htmlFor="status">
            <Trans zh="状态" en="Status" />
            <Text color="danger">*</Text>
          </Label>
          <StatusSelect name="status" required />
        </FieldValues>
        <Box alignX="right">
          <ConfirmCancel
            onClickSubmit={submitForm}
            onClickCancel={onClickCancel}
          />
        </Box>
      </Stack>
    </Form>
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
