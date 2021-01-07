import React, { useState, useEffect, useMemo } from "react"
import {
  css,
  Stack,
  Heading,
  Box,
  Button,
  SelectMenu,
  DropdownMenu
} from "bumbag"
import { useChangeBrandGameStatus } from "./client"
import Alert from "../../dialog/Alert"
import { useTrans, Trans } from "../../i18n"
import { useFormContext } from "react-hook-form"

const Enabled = "Enabled"
const EnabledTrans = {
  zh: "开启",
  en: "Enabled"
}

const Disabled = "Disabled"
const DisabledTrans = {
  zh: "关闭",
  en: "Disabled"
}

const Maintenance = "Maintenance"
const MaintenanceTrans = {
  zh: "维护",
  en: "Maintenance"
}

export const Label = ({ value = "" }) => {
  switch (value) {
    case Enabled:
      return <Trans {...EnabledTrans} />

    case Disabled:
      return <Trans {...DisabledTrans} />

    case Maintenance:
      return <Trans {...MaintenanceTrans} />

    default:
      return value
  }
}

const toPalette = (value) => {
  switch (value) {
    case Enabled:
      return "success"
    case Disabled:
      return "danger"
    case Maintenance:
      return "warning"
    default:
      return "default"
  }
}

export const StatusMenu = ({ record = {}, ...props }) => {
  let { id, status } = record
  let [newState, setSubmit] = useState()
  let {
    result,
    isLoading,
    error,
    deleteCache,
    mutate
  } = useChangeBrandGameStatus(id, newState)

  const reset = () => {
    deleteCache()
    mutate(undefined, false)
    setSubmit(undefined)
  }

  useEffect(() => {
    if (result && !error) {
      reset()
      props?.onSubmitted()
    }
  }, [result, error])

  return (
    <>
      <DropdownMenu
        menu={
          <>
            <DropdownMenu.Item onClick={() => setSubmit(Enabled)}>
              <Label value={Enabled} />
            </DropdownMenu.Item>
            <DropdownMenu.Item onClick={() => setSubmit(Disabled)}>
              <Label value={Disabled} />
            </DropdownMenu.Item>
            <DropdownMenu.Item onClick={() => setSubmit(Maintenance)}>
              <Label value={Maintenance} />
            </DropdownMenu.Item>
          </>
        }
      >
        <Button
          palette={toPalette(status)}
          isLoading={isLoading}
          disabled={isLoading}
        >
          <Label value={status} />
        </Button>
      </DropdownMenu>
      {error && <Alert {...error} onClickClose={reset} />}
    </>
  )
}

export const BrandGameStatusMenuField = ({ name, size, ...props }) => {
  let { watch } = useFormContext()
  let status = watch(name)

  return (
    <DropdownMenu
      menu={
        <>
          <DropdownMenu.Item onClick={() => setFieldValue(name, Enabled)}>
            <Label value={Enabled} />
          </DropdownMenu.Item>
          <DropdownMenu.Item onClick={() => setFieldValue(name, Disabled)}>
            <Label value={Disabled} />
          </DropdownMenu.Item>
          <DropdownMenu.Item onClick={() => setFieldValue(name, Maintenance)}>
            <Label value={Maintenance} />
          </DropdownMenu.Item>
        </>
      }
    >
      <Button palette={toPalette(status)} size={size} {...props}>
        <Label value={status} />
      </Button>
    </DropdownMenu>
  )
}
