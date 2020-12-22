import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { Alert, Button, Text, Modal } from "bumbag"
import { Delete as DeleteMenu } from "./menu"
import { useIsDisplayMenuByRoute } from "../../user/context"
import Disclosure from "../../dialog/Disclosure"
import FormDialog from "../../form/Dialog"
import { HookFormDialog } from "../../form/HookFormDialog"
import { Trans, useTrans } from "../../i18n"
import { useDeleteRole } from "./client"

export const DeleteButton = ({ id, name, onSubmitted, ...props }) => {
  let dialog = Modal.useState()

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
        />
      )}
    </>
  )
}

const DeleteDialog = ({ id, name = "", dialog, onSubmitted, ...props }) => {
  let [deleteId, setDelete] = useState()
  let { result, error, isLoading, mutate } = useDeleteRole(deleteId)

  const submit = () => {
    setDelete(id)
  }

  const clearResult = () => {
    mutate(undefined, false)
    setDelete(undefined)
  }

  let title = useTrans({
    zh: "删除权限组",
    en: "Delete Role"
  })
  return (
    <HookFormDialog
      onSubmit={submit}
      isLoading={isLoading}
      result={result}
      error={error}
      onCloseErrorAlert={clearResult}
      onSubmitted={() => {
        mutate(undefined, false)
        onSubmitted?.()
      }}
      title={title}
      dialog={dialog}
    >
      <Alert
        title={
          <Trans
            zh={`确认删除 “${name}” 权限组？`}
            en={`Confirm delete the “${name}”？`}
          />
        }
        type="warning"
        variant=""
      >
        <Text color="danger">
          <Trans
            zh="如果仍有管理员正使用此权限组，该权限组将不能够被删除"
            en="If an administrator is still using this role, it cannot be deleted"
          />
        </Text>
      </Alert>
    </HookFormDialog>
  )
}
