import React, { useState, useEffect, useMemo } from "react"
import { css, Stack, Heading, Box, Button } from "bumbag"
import { useModalState } from "../../dialog/Modal"
import { useIsDisplayMenuByRoute } from "../../user/context"
import { Update as UpdateMenu } from "./menu"
import { Disclosure } from "../../dialog/Disclosure"
import FormDialog, { ConfirmButtonFooter } from "../../form/Dialog"
import { Trans, useTrans } from "../../i18n"
import { FieldValues } from "../../layout/Grid"
import Label from "../../form/Label"
import { useUpdatePresetMaintenance } from "./client"

export const EditPresetButton = ({ record, onSubmitted, ...props }) => {
  let dialog = useModalState()
  let isDisplay = useIsDisplayMenuByRoute([UpdateMenu])
  if (!isDisplay) {
    return null
  }

  return (
    <>
      <Disclosure {...dialog}>
        <Trans zh="预设维护" en="Preset Maintenance" />
      </Disclosure>
      {dialog.visible && (
        <EditDialog record={record} onSubmitted={onSubmitted} dialog={dialog} />
      )}
    </>
  )
}

const EditDialog = ({ record, dialog, ...props }) => {
  let [submitData, setSubmit] = useState()
  let {
    result,
    isLoading,
    error,
    mutate,
    deleteCache
  } = useUpdatePresetMaintenance(record?.id, submitData)

  const reset = () => {
    deleteCache()
    mutate(undefined, false)
    setSubmit(undefined)
  }

  let title = useTrans({
    zh: "预设维护",
    en: "Preset Maintenance"
  })
  return (
    <FormDialog
      initialValues={{ ...record }}
      onSubmit={setSubmit}
      result={result}
      isLoading={isLoading}
      error={error}
      onCloseErrorAlert={reset}
      onSubmitted={() => {
        reset()
        props?.onSubmitted()
      }}
      title={title}
      showCloseButton
      showActionButtons={false}
      dialog={dialog}
      footer={<ConfirmButtonFooter />}
      maxWidth="unset"
    >
      <EditBody record={record} />
    </FormDialog>
  )
}

const EditBody = ({ record }) => {
  return <Box>TODO</Box>
}
