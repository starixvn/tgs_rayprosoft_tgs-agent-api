import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { useModalState, Stack, Heading, Text, Box, Set, Checkbox } from "bumbag"
import { useIsDisplayMenuByRoute } from "../../user/context"
import Disclosure from "../../dialog/Disclosure"
import FormDialog from "../../form/FormDialog"
import { Update as UpdateMenu } from "./menu"
import { Trans, useTrans } from "../../i18n"
import { ConfirmButtonFooter } from "../../form/ConfirmButtonFooter"
import { useAddReseller, useReseller, useUpdateReseller } from "./client"
import { FieldValues } from "../../layout/Grid"
import { NormalLabel } from "../../form/Label"
import HookFormInputField from "../../input/Input"
import { HookFormTextarea } from "../../textarea"
import { useFormContext, useFieldArray } from "react-hook-form"
import { useGameProviderList } from "../../game/providers/client"
import LoadingBackdrop from "../../dialog/LoadingBackdrop"
import AlertDialog from "../../dialog/Alert"

export const AddButton = ({ onSubmitted, ...props }) => {
  let dialog = useModalState()

  let isDisplay = useIsDisplayMenuByRoute([UpdateMenu])
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

const AddDialog = ({ onSubmitted, dialog }) => {
  let {
    data: gameProvidedrData,
    isLoading: isLoadingGameProvider,
    error: gameProviderError
  } = useGameProviderList()

  let [formData, setSubmit] = useState()
  let { result, isLoading, error, mutate } = useAddReseller()

  const onSubmit = ({ gameProviderIds, ...data }) => {
    gameProviderIds = filterMapGameProviderIds(gameProviderIds)
    let formData = {
      ...data,
      gameProviderIds
    }
    setSubmit(formData)
  }

  const reset = () => {
    mutate(undefined, false)
    setSubmit(undefined)
  }

  let title = useTrans({
    zh: "新增总公司",
    en: "Add Reseller"
  })

  if (isLoadingGameProvider) {
    return <LoadingBackdrop {...dialog} />
  }

  if (gameProviderError) {
    return <AlertDialog onClickClose={dialog.hide} />
  }

  return (
    <FormDialog
      onSubmit={onSubmit}
      result={result}
      isLoading={isLoading}
      error={gameProviderError}
      onCloseErrorAlert={reset}
      onSubmitted={() => {
        reset()
        onSubmitted?.()
      }}
      title={title}
      dialog={dialog}
      showActionButtons={false}
      footer={
        <ConfirmButtonFooter isLoading={isLoading} disabled={isLoading} />
      }
    >
      <EditBody />
    </FormDialog>
  )
}

const EditBody = ({ readonly = false }) => {
  let { register, watch, getValues, setValue, control } = useFormContext()
  const { fields, append, prepend, remove, swap, move, insert } = useFieldArray(
    {
      control,
      name: "gameProviders",
      keyName: "_id"
    }
  )

  let gameProviderIds = watch("gameProviderIds") ?? {}
  let [allGameProviderChecked, setAllGameProviderChecked] = useState()

  useEffect(() => {
    let allChecked = Object.entries(gameProviderIds).reduce(
      (allChecked, [id, checked]) => {
        if (!allChecked) {
          return allChecked
        }

        return checked
      },
      true
    )

    setAllGameProviderChecked(allChecked)
  }, [JSON.stringify(gameProviderIds)])

  let editable = !readonly

  let selectAll = useTrans({
    zh: "全选",
    en: "Select All"
  })
  return (
    <Stack orientation="vertical" width="500px">
      <Heading use="h5" marginBottom="major-1">
        <Trans zh="基本资料" en="Basic Information" />
      </Heading>
      <FieldValues>
        <NormalLabel>
          <Trans zh="公司代码" en="Code" />
        </NormalLabel>
        <Box>
          {editable && <HookFormInputField name="code" required />}
          {readonly && <Text.Block>{getValues("code")}</Text.Block>}
        </Box>

        <NormalLabel>
          <Trans zh="公司名称" en="Name" />
        </NormalLabel>
        <Box>
          {editable && <HookFormInputField name="name" required />}
          {readonly && <Text.Block>{getValues("name")}</Text.Block>}
        </Box>

        <NormalLabel>
          <Trans zh="公司简介" en="Description" />
        </NormalLabel>
        <Box>
          {editable && <HookFormTextarea name="description" />}
          {readonly && <Text.Block>{getValues("description")}</Text.Block>}
        </Box>

        <NormalLabel>
          <Trans zh="公司邮箱" en="Email" />
        </NormalLabel>
        <Box>
          {editable && (
            <HookFormInputField type="email" name="operationalEmail" required />
          )}
          {readonly && <Text.Block>{getValues("operationalEmail")}</Text.Block>}
        </Box>

        <NormalLabel>
          <Trans zh="游戏厂商" en="Game Provider" />
        </NormalLabel>
        <Set orientation="vertical">
          <Box>
            <Checkbox
              label={selectAll}
              checked={allGameProviderChecked}
              onChange={(event) => {
                let checked = event.target.checked
                fields.forEach((field, index) => {
                  setValue(`gameProviderIds.${field.id}`, checked)
                })
              }}
            />
          </Box>
          <Set>
            {fields.map((field, index) => {
              let name = `gameProviderIds.${field.id}`
              return (
                <Checkbox
                  key={field._id}
                  name={name}
                  checkboxRef={register()}
                  defaultChecked={field.checked}
                  label={field.name}
                />
              )
            })}
          </Set>
        </Set>
      </FieldValues>
    </Stack>
  )
}

export const EditButton = ({ record, onSubmitted, ...props }) => {
  let dialog = useModalState()

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
        <EditDialog record={record} onSubmitted={onSubmitted} dialog={dialog} />
      )}
    </>
  )
}

const filterMapGameProviderIds = (gameProviderIds = {}) => {
  return Object.entries(gameProviderIds).reduce(
    (gameProviderIds, [id, checked]) => {
      if (checked) {
        gameProviderIds.push(id)
      }
      return gameProviderIds
    },
    []
  )
}

const EditDialog = ({ record, onSubmitted, dialog, ...props }) => {
  let {
    data: gameProvidedrData,
    isLoading: isLoadingGameProvider,
    error: gameProviderError
  } = useGameProviderList({ showAll: true })

  let { id } = record
  let {
    data: reseller,
    isLoading: isLoadingReseller,
    error: resellerError
  } = useReseller(id)

  let [formData, setSubmit] = useState()
  let { result, isLoading, error, mutate } = useUpdateReseller(id, formData)

  const onSubmit = ({ gameProviderIds, ...data }) => {
    gameProviderIds = filterMapGameProviderIds(gameProviderIds)
    let formData = {
      ...data,
      gameProviderIds
    }
    setSubmit(formData)
  }

  const reset = () => {
    mutate(undefined, false)
    setSubmit(undefined)
  }

  let title = useTrans({
    zh: "编辑总公司",
    en: "Edit Reseller"
  })

  if (isLoadingGameProvider || isLoadingReseller) {
    return <LoadingBackdrop {...dialog} />
  }

  if (gameProviderError) {
    return <AlertDialog {...gameProviderError} onClickClose={dialog.hide} />
  }

  if (resellerError) {
    return <AlertDialog {...resellerError} onClickClose={dialog.hide} />
  }

  let gameProviders = gameProvidedrData?.items.map((gameProvider) => {
    let checked = !!reseller.gameProviders.find(
      (resellerGameProvider) => resellerGameProvider.id === gameProvider.id
    )
    return {
      ...gameProvider,
      checked
    }
  })
  return (
    <FormDialog
      defaultValues={{
        ...reseller,
        gameProviders
      }}
      onSubmit={onSubmit}
      result={result}
      isLoading={isLoadingReseller}
      error={error}
      onCloseErrorAlert={reset}
      onSubmitted={() => {
        reset()
        onSubmitted?.()
      }}
      title={title}
      dialog={dialog}
      showActionButtons={false}
      footer={
        <ConfirmButtonFooter
          isLoading={isLoadingReseller}
          disabled={isLoadingReseller}
        />
      }
      dialog={dialog}
    >
      <EditBody />
    </FormDialog>
  )
}

export const ViewButton = ({ record, ...props }) => {
  let dialog = useModalState()

  /* TODO: view auth?
  let isDisplay = useIsDisplayMenuByRoute([ViewMenu])
  if (!isDisplay) {
    return null
  }
  */
  return (
    <>
      <Disclosure {...dialog} {...props}>
        <Trans zh="查看" en="View" />
      </Disclosure>
      {dialog.visible && <ViewDialog record={record} dialog={dialog} />}
    </>
  )
}

const ViewDialog = ({ record, dialog, ...props }) => {
  let {
    data: gameProvidedrData,
    isLoading: isLoadingGameProvider,
    error: gameProviderError
  } = useGameProviderList()

  let title = useTrans({
    zh: "查看总公司",
    en: "View Reseller"
  })

  if (isLoadingGameProvider) {
    return <LoadingBackdrop {...dialog} />
  }

  if (gameProviderError) {
    return <AlertDialog onClickClose={dialog.hide} />
  }

  return (
    <FormDialog
      defaultValues={record}
      title={title}
      dialog={dialog}
      footer={<ConfirmButtonFooter />}
    >
      <EditBody />
    </FormDialog>
  )
}

const ValueBlock = ({ children, ...props }) => (
  <Text.Block alignY="center" {...props}>
    {children}
  </Text.Block>
)
