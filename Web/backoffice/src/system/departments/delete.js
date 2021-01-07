import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { Alert, Button, Modal } from "bumbag"
import { Delete as DeleteMenu } from "./menu"
import { useIsDisplayMenuByRoute } from "../../user/context"
import Disclosure from "../../dialog/Disclosure"
import { Trans, useTrans } from "../../i18n"
import { useDeleteDepartment } from "./client"
import FormDialog from "../../form/FormDialog"

export const DeleteButton = ({ id, name, onSubmitted, ...props }) => {
  let dialog = Modal.useState()
  let disclosureRef = useRef()

  let isDisplay = useIsDisplayMenuByRoute([DeleteMenu])
  if (!isDisplay) {
    return null
  }

  return (
    <>
      <Disclosure palette="danger" {...dialog}>
        <Trans zh="删除" en="Delete" />
      </Disclosure>
      {dialog.visible && (
        <DeleteDialog
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

const DeleteDialog = ({
  id,
  name = "",
  dialog,
  onSubmitted,
  finalFocusRef,
  ...props
}) => {
  let [deleteId, setDelete] = useState()
  let { result, error, isLoading } = useDeleteDepartment(deleteId)

  const submit = () => setDelete(id)

  const clearResult = () => {
    setDelete(null)
  }

  let title = useTrans({
    zh: "删除部门",
    en: "Delete Department"
  })

  return (
    <FormDialog
      onSubmit={submit}
      isLoading={isLoading}
      result={result}
      error={error}
      onCloseErrorAlert={clearResult}
      onSubmitted={onSubmitted}
      title={title}
      dialog={dialog}
    >
      <Alert
        title={
          <Trans
            zh={`确认删除 “${name}” 部门`}
            en={`Confirm delete the “${name}”？`}
          />
        }
        type="warning"
        variant=""
      ></Alert>
    </FormDialog>
  )
}
