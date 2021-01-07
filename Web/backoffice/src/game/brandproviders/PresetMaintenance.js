import React, { useState, useEffect, useMemo } from "react"
import {
  css,
  Stack,
  Heading,
  Box,
  Button,
  Group,
  Set,
  Table,
  DropdownMenu
} from "bumbag"
import { useModalState } from "../../dialog/Modal"
import { useIsDisplayMenuByRoute } from "../../user/context"
import { Update as UpdateMenu } from "../providers/menu"
import { Disclosure } from "../../dialog/Disclosure"
import { ConfirmButtonFooter } from "../../form/ConfirmButtonFooter"
import { Trans, useTrans } from "../../i18n"
import { FieldValues } from "../../layout/Grid"
import Label from "../../form/Label"
import {
  useUpdateBrandGameProviderPresetMaintenance,
  useBrandGameProviderPresetMaintenanceList
} from "./client"
import FormDialog from "../../form/FormDialog"
import LoadingBackdrop from "../../dialog/LoadingBackdrop"
import AlertDialog from "../../dialog/Alert"
import DateTimeInput from "../../date/DateTimeInput"
import cloneDeep from "clone-deep"
import {
  Controller,
  FormProvider,
  useForm,
  useFormContext
} from "react-hook-form"
import format from "../../date/format"
import { ScheduleStatus, ScheduleStatusLabel } from "./ScheduleStatus"

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
  let { id } = record
  let {
    items,
    isLoading: isLoadingPresetMaintenance,
    error: presetMaintenanceError,
    deleteCache
  } = useBrandGameProviderPresetMaintenanceList(id)

  let [defaultTimes, setDefaultTimes] = useState([])

  const add = (record) => {
    setDefaultTimes((list) => [...list, record])
  }

  const deletePresetMaintenance = (index) => {
    setDefaultTimes((list) => {
      list.splice(index, 1)
      return [...list]
    })
  }

  const setScheduleStatus = (scheduleStatus) => {
    setDefaultTimes((list) =>
      list.map((item) => ({
        ...item,
        scheduleStatus
      }))
    )
  }

  useEffect(() => {
    if (items?.length) {
      setDefaultTimes(cloneDeep(items))
    }
  }, [items])

  let [submitData, setSubmit] = useState()
  let {
    result,
    isLoading,
    error,
    mutate
  } = useUpdateBrandGameProviderPresetMaintenance(record?.id, submitData)

  const onSubmit = (formData) => {
    setSubmit({ defaultTimes, scheduleStatus: formData.scheduleStatus })
  }

  const reset = () => {
    deleteCache()
    mutate(undefined, false)
    setSubmit(undefined)
  }

  let title = useTrans({
    zh: "预设维护",
    en: "Preset Maintenance"
  })

  if (isLoadingPresetMaintenance) {
    return <LoadingBackdrop {...dialog} />
  }

  if (presetMaintenanceError) {
    return (
      <AlertDialog {...presetMaintenanceError} onClickClose={dialog.hide} />
    )
  }

  return (
    <FormDialog
      defaultValues={{
        scheduleStatus: "",
        from: null,
        to: null
      }}
      onSubmit={onSubmit}
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
      footer={
        <ConfirmButtonFooter isLoading={isLoading} disabled={isLoading} />
      }
      placement="top"
      minWidth="60vw"
      maxWidth="unset"
    >
      <EditBody
        setScheduleStatus={setScheduleStatus}
        items={defaultTimes}
        add={add}
        deletePresetMaintenance={deletePresetMaintenance}
      />
    </FormDialog>
  )
}

const StartTimeLabel = () => <Trans zh="开始时间" en="Start Time" />
const EndTimeLabel = () => <Trans zh="结束时间" en="End Time" />

const EditBody = ({
  items,
  add,
  deletePresetMaintenance,
  setScheduleStatus
}) => {
  return (
    <Stack minHeight="50vh">
      <AddPresetMaintenance add={add} />
      <ScheduleStatusDropdownMenu
        items={items}
        setScheduleStatus={setScheduleStatus}
      />
      <Table>
        <Table.Head>
          <Table.Row>
            <Table.HeadCell textAlign="center">
              <StartTimeLabel />
            </Table.HeadCell>
            <Table.HeadCell textAlign="center">
              <EndTimeLabel />
            </Table.HeadCell>
            <Table.HeadCell textAlign="center">
              <Trans zh="操作" en="Actions" />
            </Table.HeadCell>
          </Table.Row>
        </Table.Head>
        <Table.Body>
          {items.map((item, index, list) => (
            <PresetMaintenanceRow
              key={index}
              {...item}
              index={index}
              deletePresetMaintenance={deletePresetMaintenance}
            />
          ))}
        </Table.Body>
      </Table>
    </Stack>
  )
}

const PresetMaintenanceRow = ({
  from,
  to,
  scheduleStatus,
  index,
  deletePresetMaintenance
}) => {
  return (
    <Table.Row>
      <Table.Cell textAlign="center">{format(from)}</Table.Cell>
      <Table.Cell textAlign="center">{format(to)}</Table.Cell>
      <Table.Cell textAlign="center">
        <Button
          palette="danger"
          onClick={() => {
            deletePresetMaintenance(index)
          }}
          visibility={
            scheduleStatus === ScheduleStatus.Enabled ? "visible" : "hidden"
          }
        >
          <Trans zh="删除" en="Delete" />
        </Button>
      </Table.Cell>
    </Table.Row>
  )
}

const AddPresetMaintenance = ({ add }) => {
  let { watch, errors, setValue, getValues, trigger } = useFormContext()
  let from = watch("from")
  let minDate = watch("from")

  let to = watch("to")
  let maxDate = watch("to")
  let scheduleStatus = watch("scheduleStatus")
  return (
    <Set orientation="horizontal">
      <Group>
        <Button isStatic>
          <StartTimeLabel />
        </Button>
        <DateTimeInput name="from" maxDate={maxDate} />
      </Group>

      <Group>
        <Button isStatic>
          <EndTimeLabel />
        </Button>
        <DateTimeInput name="to" minDate={minDate} />
      </Group>

      <Button
        palette="primary"
        onClick={(event) => {
          //TODO: trigger extra validation? focus on input?
          if (from && to) {
            add({
              from,
              to,
              scheduleStatus
            })
            setValue("from", null)
            setValue("to", null)
          }
        }}
        disabled={!from || !to}
      >
        <Trans zh="新增" en="Add" />
      </Button>
    </Set>
  )
}

const ScheduleStatusDropdownMenu = ({
  name = "scheduleStatus",
  items,
  setScheduleStatus
}) => {
  let { setValue, watch, getValues, control } = useFormContext()
  useEffect(() => {
    let result = items.reduce(
      ({ isSame, scheduleStatus }, item, index) => {
        if (!isSame) {
          return {
            isSame,
            scheduleStatus
          }
        }

        if (index === 0) {
          //init
          return {
            isSame,
            scheduleStatus: item.scheduleStatus
          }
        }

        if (item.scheduleStatus !== scheduleStatus) {
          return {
            isSame: false
          }
        }

        return {
          isSame,
          scheduleStatus
        }
      },
      {
        isSame: true,
        scheduleStatus: undefined
      }
    )

    if (result.isSame) {
      setValue(name, result.scheduleStatus ?? ScheduleStatus.Enabled)
    }
  }, [JSON.stringify(items)])

  return (
    <Controller
      control={control}
      name={name}
      render={(
        { onChange, onBlur, value, name, ref },
        { invalid, isTouched, isDirty }
      ) => {
        return (
          <DropdownMenu
            menu={
              <>
                {value != ScheduleStatus.Enabled && (
                  <DropdownMenu.Item
                    onClick={() => {
                      setScheduleStatus(ScheduleStatus.Enabled)
                      setValue(name, ScheduleStatus.Enabled)
                    }}
                  >
                    <ScheduleStatusLabel value={ScheduleStatus.Enabled} />
                  </DropdownMenu.Item>
                )}
                {value != ScheduleStatus.Disabled && (
                  <DropdownMenu.Item
                    onClick={() => {
                      setScheduleStatus(ScheduleStatus.Disabled)
                      setValue(name, ScheduleStatus.Disabled)
                    }}
                  >
                    <ScheduleStatusLabel value={ScheduleStatus.Disabled} />
                  </DropdownMenu.Item>
                )}
              </>
            }
          >
            <Group>
              <Button isStatic={!!value}>
                <Trans zh="状态" en="Status" />
              </Button>
              {value && (
                <Button>
                  <ScheduleStatusLabel value={value} />
                </Button>
              )}
            </Group>
          </DropdownMenu>
        )
      }}
    />
  )
}
