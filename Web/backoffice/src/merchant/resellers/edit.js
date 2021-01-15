import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import {
  useModalState,
  Stack,
  Heading,
  Text,
  Box,
  Set,
  FieldStack
} from "bumbag"
import { useIsDisplayMenuByRoute } from "../../user/context"
import Disclosure from "../../dialog/Disclosure"
import FormDialog from "../../form/FormDialog"
import { Update as UpdateMenu } from "./menu"
import { Trans, useTrans } from "../../i18n"
import {
  CloseButtonFooter,
  ConfirmButtonFooter
} from "../../form/ConfirmButtonFooter"
import { useAddReseller, useReseller, useUpdateReseller } from "./client"
import { FieldValues } from "../../layout/Grid"
import { NormalLabel } from "../../form/Label"
import Input from "../../input/Input"
import Textarea from "../../textarea"
import { useFormContext, useFieldArray } from "react-hook-form"
import { useGameProviderList } from "../../game/providers/client"
import LoadingBackdrop from "../../dialog/LoadingBackdrop"
import AlertDialog from "../../dialog/Alert"
import { BusinessTypeRadioGroup } from "../BusinessType"
import { BetLimitCheckboxGroup } from "../../betlimit/BetLimitCheckboxGroup"
import { useBetLimitList } from "../../betlimit/client"
import { GameProviderCheckboxGroup } from "../../game/providers/GameProviderCheckboxGroup"
import { allCheckedKeys } from "../../checkbox/allCheckedKeys"
import {
  mapChecked,
  mapCheckedByCode,
  mapCheckedByCultureCode,
  mapCheckedById
} from "../../checkbox/mapChecked"
import { CurrencyCheckboxGroupTable } from "../../currency/CurrencyCheckboxGroup"
import { LanguageCheckboxGroupTable } from "../../language/LanguageCheckboxGroup"
import { useLanguageList } from "../../language/client"
import { useCurrencyList } from "../../currency/client"

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
        <LoadingDataDialog>
          {(dataProps) => (
            <AddDialog
              onSubmitted={onSubmitted}
              dialog={dialog}
              {...dataProps}
            />
          )}
        </LoadingDataDialog>
      )}
    </>
  )
}

const normalizeFormData = ({
  cultureCodes,
  currencyCodes,
  gameProviderIds,
  betLimitIds,
  ...data
} = {}) => {
  cultureCodes = allCheckedKeys(cultureCodes)
  currencyCodes = allCheckedKeys(currencyCodes)
  gameProviderIds = allCheckedKeys(gameProviderIds)
  betLimitIds = allCheckedKeys(betLimitIds)
  return {
    ...data,
    cultureCodes,
    currencyCodes,
    gameProviderIds,
    betLimitIds
  }
}

const formDataOptionLoadings = () => [
  ["languageData", useLanguageList({ showAll: true })],
  ["currencyData", useCurrencyList({ showAll: true })],
  ["gameProviderData", useGameProviderList({ showAll: true })],
  ["betLimitData", useBetLimitList({ showAll: true })]
]

const LoadingDataDialog = ({
  loadings = formDataOptionLoadings(),
  dialog,
  children
}) => {
  let isLoading = loadings.find(([name, { isLoading }]) => isLoading) ?? false

  if (isLoading) {
    return <LoadingBackdrop {...dialog} />
  }

  let error = loadings.find(([name, { error }]) => !!error)

  if (error) {
    return <AlertDialog {...error} onClickClose={dialog.hide} />
  }

  let dataProps = loadings.reduce((results, [name, { data }]) => {
    Reflect.set(results, name, data)
    return results
  }, {})
  return children(dataProps)
}

const AddDialog = ({
  languageData,
  currencyData,
  gameProviderData,
  betLimitData,
  onSubmitted,
  dialog
}) => {
  let [formData, setSubmit] = useState()
  let { result, isLoading, error, mutate } = useAddReseller(formData)

  const onSubmit = (data) => {
    setSubmit(normalizeFormData(data))
  }

  const reset = () => {
    mutate(undefined, false)
    setSubmit(undefined)
  }

  let title = useTrans({
    zh: "新增总公司",
    en: "Add Reseller"
  })

  let languages = mapCheckedByCultureCode([], languageData?.items)
  let currencies = mapCheckedByCode([], currencyData?.items)
  let gameProviders = mapCheckedById([], gameProviderData?.items)
  let betLimits = mapCheckedById([], betLimitData?.items)
  return (
    <FormDialog
      defaultValues={{
        languages,
        currencies,
        gameProviders,
        betLimits
      }}
      onSubmit={onSubmit}
      result={result}
      isLoading={isLoading}
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
        <ConfirmButtonFooter isLoading={isLoading} disabled={isLoading} />
      }
      placement="top"
      minWidth="60vw"
      maxWidth="unset"
      css={css`
        & .bb-DialogContent {
          position: relative;
        }
      `}
    >
      <EditBody isAdd={true} />
    </FormDialog>
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
        <LoadingDataDialog>
          {(dataProps) => (
            <EditDialog
              record={record}
              onSubmitted={onSubmitted}
              dialog={dialog}
              {...dataProps}
            />
          )}
        </LoadingDataDialog>
      )}
    </>
  )
}

const EditDialog = ({
  languageData,
  currencyData,
  gameProviderData,
  betLimitData,
  record,
  onSubmitted,
  dialog,
  ...props
}) => {
  let { id } = record
  let {
    data: reseller,
    isLoading: isLoadingReseller,
    error: resellerError,
    mutate: mutateReseller,
    deleteCache: deleteResellerCache
  } = useReseller(id)

  let [formData, setSubmit] = useState()
  let { result, isLoading, error, mutate } = useUpdateReseller(id, formData)

  const onSubmit = (data) => {
    setSubmit({
      ...normalizeFormData(data),
      code: record.code
    })
  }

  const reset = () => {
    mutateReseller(undefined, false)
    deleteResellerCache()

    mutate(undefined, false)
    setSubmit(undefined)
  }

  let title = useTrans({
    zh: "编辑总公司",
    en: "Edit Reseller"
  })

  if (isLoadingReseller) {
    return <LoadingBackdrop {...dialog} />
  }

  if (resellerError) {
    return <AlertDialog {...resellerError} onClickClose={dialog.hide} />
  }

  let languages = mapCheckedByCultureCode(
    reseller.languages,
    languageData?.items
  )
  let currencies = mapCheckedByCode(reseller.currencies, currencyData?.items)
  let gameProviders = mapCheckedById(
    reseller.gameProviders,
    gameProviderData?.items
  )
  let betLimits = mapCheckedById(reseller.betLimits, betLimitData?.items)
  return (
    <FormDialog
      defaultValues={{
        ...reseller,
        languages,
        currencies,
        gameProviders,
        betLimits
      }}
      onSubmit={onSubmit}
      result={result}
      isLoading={isLoading}
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
        <ConfirmButtonFooter isLoading={isLoading} disabled={isLoading} />
      }
      dialog={dialog}
      placement="top"
      minWidth="60vw"
      maxWidth="unset"
      css={css`
        & .bb-DialogContent {
          position: relative;
        }
      `}
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
      {dialog.visible && (
        <LoadingDataDialog>
          {(dataProps) => (
            <ViewDialog id={record.id} dialog={dialog} {...dataProps} />
          )}
        </LoadingDataDialog>
      )}
    </>
  )
}

const ViewDialog = ({
  languageData,
  currencyData,
  gameProviderData,
  betLimitData,
  id,
  dialog,
  ...props
}) => {
  let {
    data: reseller,
    isLoading: isLoadingReseller,
    error: resellerError
  } = useReseller(id)

  let title = useTrans({
    zh: "查看总公司",
    en: "View Reseller"
  })

  if (isLoadingReseller) {
    return <LoadingBackdrop {...dialog} />
  }

  if (resellerError) {
    return <AlertDialog {...resellerError} onClickClose={dialog.hide} />
  }

  let languages = mapCheckedByCultureCode(
    reseller.languages,
    languageData?.items
  )
  let currencies = mapCheckedByCode(reseller.currencies, currencyData?.items)
  let gameProviders = mapCheckedById(
    reseller.gameProviders,
    gameProviderData?.items
  )
  let betLimits = mapCheckedById(reseller.betLimits, betLimitData?.items)

  let record = {
    ...reseller,
    languages,
    currencies,
    gameProviders,
    betLimits
  }
  return (
    <FormDialog
      defaultValues={record}
      title={title}
      dialog={dialog}
      showActionButtons={false}
      footer={<CloseButtonFooter dialog={dialog} />}
      placement="top"
      minWidth="60vw"
      maxWidth="unset"
      css={css`
        & .bb-DialogContent {
          position: relative;
        }
      `}
    >
      <EditBody record={record} readonly />
    </FormDialog>
  )
}

const EditBody = ({ record = {}, readonly = false, isAdd = false }) => {
  let { register, watch, getValues, setValue, control } = useFormContext()

  let editable = !readonly
  return (
    <Stack orientation="vertical">
      <Heading use="h5" marginBottom="major-1">
        <Trans zh="基本资料" en="Basic Information" />
      </Heading>
      <FieldValues>
        <NormalLabel>
          <Trans zh="商户类型" en="Type" />
        </NormalLabel>
        <Box alignY="center">
          <BusinessTypeRadioGroup disabled={readonly || !isAdd} />
        </Box>

        <NormalLabel>
          <Trans zh="总公司代码" en="Code" />
        </NormalLabel>
        <Box alignY="center">
          {editable && <Input name="code" required disabled={!isAdd} />}
          {readonly && <Text.Block>{record?.code}</Text.Block>}
        </Box>

        <NormalLabel>
          <Trans zh="总公司名称" en="Name" />
        </NormalLabel>
        <Box alignY="center">
          {editable && <Input name="name" required />}
          {readonly && <Text.Block>{record?.name}</Text.Block>}
        </Box>

        <NormalLabel>
          <Trans zh="总公司简介" en="Description" />
        </NormalLabel>
        <Box alignY="center">
          {editable && <Textarea name="description" />}
          {readonly && <Text.Block>{record.description}</Text.Block>}
        </Box>

        <NormalLabel>
          <Trans zh="总公司邮箱" en="Email" />
        </NormalLabel>
        <Box alignY="center">
          {editable && <Input type="email" name="operationalEmail" required />}
          {readonly && <Text.Block>{record.operationalEmail}</Text.Block>}
        </Box>

        <NormalLabel>
          <Trans zh="语言" en="Language" />
        </NormalLabel>
        <LanguageCheckboxGroupTable marginTop="major-1" disabled={readonly} />

        <NormalLabel>
          <Trans zh="币种" en="Currency" />
        </NormalLabel>
        <CurrencyCheckboxGroupTable marginTop="major-1" disabled={readonly} />

        <NormalLabel>
          <Trans zh="游戏厂商" en="Game Provider" />
        </NormalLabel>
        <GameProviderCheckboxGroup
          name="gameProviderIds"
          fieldArrayName="gameProviders"
          marginTop="major-1"
          disabled={readonly}
        />

        <NormalLabel>
          <Trans zh="游戏商限红" en="Game Provider Bet Limit" />
        </NormalLabel>
        <FieldStack marginTop="major-1">
          <BetLimitCheckboxGroup disabled={readonly} />
        </FieldStack>
      </FieldValues>
    </Stack>
  )
}

const ValueBlock = ({ children, ...props }) => (
  <Text.Block alignY="center" {...props}>
    {children}
  </Text.Block>
)
