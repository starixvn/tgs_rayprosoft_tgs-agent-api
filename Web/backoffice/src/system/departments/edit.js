import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { Stack, Modal, Box } from "bumbag"
import { Create as CreateMenu, Update as UpdateMenu } from "./menu"
import { useIsDisplayMenuByRoute } from "../../user/context"
import { Trans, useTrans } from "../../i18n"
import { useAddDepartment, useEditDepartment } from "./client"
import Label from "../../form/Label"
import { FieldValues } from "../../layout/Grid"
import Disclosure from "../../dialog/Disclosure"
import { useForm, useFormContext, FormProvider } from "react-hook-form"
import FormDialog from "../../form/FormDialog"
import HookFormInputField from "../../input/Input"

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

const AddDialog = ({ onSubmitted, dialog, ...props }) => {
  let [submitData, setSubmit] = useState()
  let { result, error, isLoading } = useAddDepartment(submitData)

  const clearResult = () => {
    setSubmit(null)
  }

  let title = useTrans({
    zh: "新增部门",
    en: "Add Department"
  })

  return (
    <FormDialog
      defaultValues={{
        name: ""
      }}
      onSubmit={setSubmit}
      isLoading={isLoading}
      result={result}
      error={error}
      onCloseErrorAlert={clearResult}
      onSubmitted={onSubmitted}
      title={title}
      dialog={dialog}
    >
      <EditBody />
    </FormDialog>
  )
}

const EditBody = () => (
  <Stack spacing="major-3" margin="major-1">
    <FieldValues>
      <Label name="name" label={<Trans zh="部门名称" en="Department Name" />} />
      <Box>
        <HookFormInputField name="name" required />
      </Box>
    </FieldValues>
  </Stack>
)

export const EditButton = ({ id = "", name = "", onSubmitted, ...props }) => {
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
        <EditDialog
          id={id}
          name={name}
          onSubmitted={onSubmitted}
          dialog={dialog}
        />
      )}
    </>
  )
}

const EditDialog = ({ id, name = "", dialog, onSubmitted, ...props }) => {
  let [submitData, setSubmit] = useState()
  let { result, error, isLoading } = useEditDepartment(id, submitData)

  const clearResult = () => {
    setSubmit(null)
  }

  let title = useTrans({
    zh: "修改部门",
    en: "Edit Department"
  })

  return (
    <FormDialog
      defaultValues={{
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
    >
      <EditBody />
    </FormDialog>
  )
}
