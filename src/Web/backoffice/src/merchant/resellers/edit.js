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
import { useBetLimitList } from "../../betlimit/client"
import { toCheckedGameProviderIds } from "../../game/providers/GameProviderCheckboxGroup"
import {
  mapChecked,
  mapCheckedByCode,
  mapCheckedByCultureCode,
  mapCheckedById
} from "../../checkbox/mapChecked"
import { useLanguageList } from "../../language/client"
import { useCurrencyList } from "../../currency/client"
import { CurrencyFieldsBox, toCheckedCurrencyCodes } from "../CurrencyFieldsBox"
import { LanguageFieldsBox, toCheckedCultureCodes } from "../LanguageFieldsBox"
import { BetLimitFieldsBox, toCheckedBetLimitIds } from "../BetLimitFieldsBox"
import GameProviderFieldsBox from "../GameProviderFieldsBox"

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

const normalizeFormData = ({
  languages,
  currencies,
  gameProviders,
  betLimits,
  ...data
} = {}) => {
  let cultureCodes = toCheckedCultureCodes(languages)

  let currencyCodes = toCheckedCurrencyCodes(currencies)

  let gameProviderIds = toCheckedGameProviderIds(gameProviders)

  let betLimitIds = toCheckedBetLimitIds(betLimits)

  return {
    ...data,
    cultureCodes,
    currencyCodes,
    gameProviderIds,
    betLimitIds
  }
}

const AddDialog = ({ onSubmitted, dialog }) => {
  let {
    languages,
    currencies,
    gameProviders,
    betLimits,
    isLoading: isLoadingData,
    error: dataError,
    muateGameProvider,
    deleteGameProviderCache
  } = useResellerData()
  let [formData, setSubmit] = useState()
  let { result, isLoading, error, mutate } = useAddReseller(formData)

  const onSubmit = (data) => {
    setSubmit(normalizeFormData(data))
  }

  const reset = () => {
    muateGameProvider(undefined, false)
    deleteGameProviderCache()

    mutate(undefined, false)
    setSubmit(undefined)
  }

  let title = useTrans({
    zh: "新增总公司",
    en: "Add Reseller"
  })

  if (isLoadingData) {
    return <LoadingBackdrop {...dialog} />
  }

  if (dataError) {
    return <AlertDialog {...dataError} onClickClose={dialog.hide} />
  }

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
      onClickClose={reset}
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
        <EditDialog record={record} onSubmitted={onSubmitted} dialog={dialog} />
      )}
    </>
  )
}

const EditDialog = ({ record, onSubmitted, dialog, ...props }) => {
  let { id } = record
  let {
    reseller,
    languages,
    currencies,
    gameProviders,
    betLimits,
    isLoading: isLoadingReseller,
    error: resellerError,
    mutate: mutateReseller,
    deleteCache: deleteResellerCache
  } = useResellerData(id)

  let [formData, setSubmit] = useState()
  let { result, isLoading, error, mutate } = useUpdateReseller(id, formData)

  const onSubmit = (data) => {
    setSubmit(normalizeFormData(data))
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
      onClickClose={reset}
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
      {dialog.visible && <ViewDialog id={record.id} dialog={dialog} />}
    </>
  )
}

const useResellerData = (id) => {
  let {
    data: reseller,
    isLoading: isLoadingReseller,
    error: resellerError,
    mutate,
    deleteCache
  } = useReseller(id)
  let {
    data: languageData,
    isLoading: isLoadingLanguage,
    error: languageError,
    mutate: muateLanguage,
    deleteCache: deleteLanguageCache
  } = useLanguageList({ showAll: true })
  let {
    data: currencyData,
    isLoading: isLoadingCurrency,
    error: currencyError,
    mutate: muateCurrency,
    deleteCache: deleteCurrencyCache
  } = useCurrencyList({ showAll: true })
  let {
    data: gameProviderData,
    isLoading: isLoadingGameProvider,
    error: gameProviderError,
    mutate: muateGameProvider,
    deleteCache: deleteGameProviderCache
  } = useGameProviderList({ showAll: true })
  let {
    data: betLimitData,
    isLoading: isLoadingBetLimit,
    error: betLimitError,
    mutate: muateBetLimit,
    deleteCache: deleteBetLimitCache
  } = useBetLimitList({ showAll: true })

  return {
    reseller,
    languages: mapCheckedByCultureCode(
      reseller?.languages ?? [],
      languageData?.items ?? []
    ),
    currencies: mapCheckedByCode(
      reseller?.currencies ?? [],
      currencyData?.items ?? []
    ),
    gameProviders: mapCheckedById(
      reseller?.gameProviders ?? [],
      gameProviderData?.items ?? []
    ),
    betLimits: mapCheckedById(reseller?.betLimits ?? [], betLimitData?.items),
    isLoading:
      isLoadingReseller ||
      isLoadingLanguage ||
      isLoadingCurrency ||
      isLoadingGameProvider ||
      isLoadingBetLimit,
    error:
      resellerError ||
      languageError ||
      currencyError ||
      gameProviderError ||
      betLimitError,
    mutate,
    muateGameProvider,
    deleteCache,
    deleteGameProviderCache
  }
}

const ViewDialog = ({ id, dialog, ...props }) => {
  let {
    reseller,
    languages,
    currencies,
    gameProviders,
    betLimits,
    isLoading,
    error,
    mutate,
    deleteCache
  } = useResellerData(id)

  let title = useTrans({
    zh: "查看总公司",
    en: "View Reseller"
  })

  if (isLoading) {
    return <LoadingBackdrop {...dialog} />
  }

  if (error) {
    return <AlertDialog {...error} onClickClose={dialog.hide} />
  }

  const reset = () => {
    mutate(undefined, false)
    deleteCache()
  }

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
      onClickClose={reset}
      showActionButtons={false}
      footer={<CloseButtonFooter dialog={dialog} onClickClose={reset} />}
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
          {editable && <BusinessTypeRadioGroup disabled={readonly || !isAdd} />}
          {readonly && <Text.Block>{record?.businessType}</Text.Block>}
        </Box>

        <NormalLabel>
          <Trans zh="总公司代码" en="Code" />
        </NormalLabel>
        <Box alignY="center">
          <Input
            name="code"
            type={isAdd ? "input" : "hidden"}
            required={isAdd}
          />
          {!isAdd && <Text.Block>{getValues("code")}</Text.Block>}
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

        <LanguageFieldsBox readonly={readonly} />

        <CurrencyFieldsBox readonly={readonly} />

        <GameProviderFieldsBox readonly={readonly} />

        <BetLimitFieldsBox readonly={readonly} />
      </FieldValues>
    </Stack>
  )
}
