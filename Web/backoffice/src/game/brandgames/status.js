import React, { useState, useEffect, useMemo } from "react"
import {
  css,
  Stack,
  Heading,
  Box,
  Button,
  Select as _Select,
  SelectMenu,
  DropdownMenu
} from "bumbag"
import { useFormikContext } from "formik"
import { useUpdateBrandGameStatus } from "./client"
import Alert from "../../dialog/Alert"
import { Field } from "../../select/Field"
import { useTrans, Trans } from "../../i18n"

const Enabled = 0
const EnabledName = "Enabled"
const EnabledTrans = {
  zh: "开启",
  en: "Enabled"
}

const Disabled = 1
const DisabledName = "Disabled"
const DisabledTrans = {
  zh: "关闭",
  en: DisabledName
}

const Maintenance = 2
const MaintenanceName = "Maintenance"
const MaintenanceTrans = {
  zh: "维护",
  en: MaintenanceName
}

export const Label = ({ value = "" }) => {
  switch (value) {
    case Enabled:
    case EnabledName:
      return <Trans {...EnabledTrans} />

    case Disabled:
    case DisabledName:
      return <Trans {...DisabledTrans} />

    case Maintenance:
    case MaintenanceName:
      return <Trans {...MaintenanceTrans} />

    default:
      return value
  }
}

const toPalette = (value) => {
  switch (value) {
    case EnabledName:
      return "success"
    case DisabledName:
      return "danger"
    case MaintenanceName:
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
  } = useUpdateBrandGameStatus(id, newState)

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
            <DropdownMenu.Item onClick={() => setSubmit(EnabledName)}>
              <Label value={EnabledName} />
            </DropdownMenu.Item>
            <DropdownMenu.Item onClick={() => setSubmit(DisabledName)}>
              <Label value={DisabledName} />
            </DropdownMenu.Item>
            <DropdownMenu.Item onClick={() => setSubmit(MaintenanceName)}>
              <Label value={MaintenanceName} />
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

export const StatusMenuField = ({ name, size, ...props }) => {
  let { values, setFieldValue } = useFormikContext()
  let status = Reflect.get(values, name)

  return (
    <DropdownMenu
      menu={
        <>
          <DropdownMenu.Item onClick={() => setFieldValue(name, EnabledName)}>
            <Label value={EnabledName} />
          </DropdownMenu.Item>
          <DropdownMenu.Item onClick={() => setFieldValue(name, DisabledName)}>
            <Label value={DisabledName} />
          </DropdownMenu.Item>
          <DropdownMenu.Item
            onClick={() => setFieldValue(name, MaintenanceName)}
          >
            <Label value={MaintenanceName} />
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
