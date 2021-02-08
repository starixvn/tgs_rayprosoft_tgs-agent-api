import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import {
  useModalState,
  Stack,
  Heading,
  Box,
  Text,
  FieldStack,
  Card,
  Set,
  Table
} from "bumbag"
import { useIsDisplayMenuByRoute, useUserLicenseeId } from "../../user/context"
import { Disclosure, ConfigIconDisclosure } from "../../dialog/Disclosure"
import FormDialog from "../../form/FormDialog"
import { Update as UpdateMenu } from "./menu"
import { useBrand, useUpdateBrand } from "./client"
import { getTrans, Trans, useLocale, useTrans } from "../../i18n"
import ConfirmButtonFooter from "../../form/ConfirmButtonFooter"
import {
  useFormContext,
  useFieldArray,
  Controller,
  useForm
} from "react-hook-form"
import LoadingBackdrop from "../../dialog/LoadingBackdrop"
import AlertDialog from "../../dialog/Alert"
import { FieldValues } from "../../layout/Grid"
import { Input, InputField } from "../../input/Input"
import { TextareaField } from "../../textarea"
import { Switch, SwitchField } from "../../switch/Switch"
import useLicenseeBrand from "./useLicenseeBrand"
import Backdrop from "../../dialog/Backdrop"
import { useGameProviderBetLimitList } from "../../game/providers/betlimits/client"
import { Select } from "../../select/Select"
import { useGameProvider } from "../../game/providers/client"
import { Checkbox } from "../../checkbox"
import cloneDeep from "clone-deep"

export const EditBrandBetLimitGameProviderBetLimit = ({
  record,
  onSubmitted,
  ...props
}) => {
  let dialog = useModalState()

  let isDisplay = useIsDisplayMenuByRoute([UpdateMenu])
  if (!isDisplay) {
    return null
  }

  return (
    <>
      <Disclosure {...dialog}>
        <Trans zh="限红组" en="Bet Limit Group" />
      </Disclosure>
      {dialog.visible && (
        <EditDialog
          brandId={record?.id}
          licenseeId={record?.licenseeId}
          onSubmitted={onSubmitted}
          dialog={dialog}
        />
      )}
    </>
  )
}

const EditDialog = ({ brandId = "", licenseeId = "", onSubmitted, dialog }) => {
  let {
    brand,
    gameProviders,
    betLimits,
    isLoading: isLoadingBrand,
    error: brandError,
    mutateBrand,
    deleteBrandCache
  } = useLicenseeBrand({ brandId, licenseeId })

  gameProviders = gameProviders.filter(({ checked }) => checked)
  betLimits = betLimits.filter(({ checked }) => checked)

  let [formData, setSubmit] = useState()
  let { result, isLoading, error, mutate } = useUpdateBrand(brandId, formData)

  let title = useTrans({
    zh: "限红组",
    en: "Bet Limit Group"
  })

  if (isLoadingBrand) {
    return <LoadingBackdrop {...dialog} />
  }

  if (brandError) {
    return (
      <AlertDialog {...brandError} {...dialog} onClickClose={dialog.hide} />
    )
  }

  const onSubmit = (data) => {
    let formData = normalizeFormData({ brand, ...data })
    setSubmit(formData)
  }

  const reset = () => {
    mutateBrand(undefined, false)
    deleteBrandCache()

    setSubmit(undefined)
    mutate(undefined, false)
  }

  gameProviders = gameProviders.map((gameProvider) => {
    // all games inside game pool
    let games =
      brand?.games.filter(
        (game) => game.gameProviderId === gameProvider.gameProviderId
      ) ?? []

    return {
      ...gameProvider,
      games
    }
  })

  return (
    <FormDialog
      defaultValues={{
        ...brand,
        brandGroups: normalizeBrandGroups(brand?.brandGroups)
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
      showActionButtons={false}
      footer={
        <ConfirmButtonFooter isLoading={isLoading} disabled={isLoading} />
      }
      dialog={dialog}
      placement="top"
      minWidth="60vw"
      maxWidth="90vw"
      css={css`
        & .bb-DialogContent {
          position: relative;
        }
      `}
    >
      <EditBody
        brand={brand}
        betLimits={betLimits}
        gameProviders={gameProviders}
      />
    </FormDialog>
  )
}

const normalizeBrandGroups = (brandGroups = []) => {
  return (
    brandGroups.reduce((result, brandGroup, index) => {
      let { gameProviderBetLimits } = brandGroup
      let betLimitGameProviderPairs = groupByBetLimit(gameProviderBetLimits)
      result.push({
        ...brandGroup,
        betLimitGameProviderPairs
      })
      return result
    }, []) ?? []
  )
}

const normalizeFormData = ({ brand = {}, brandGroups = [], ...data }) => {
  let currencyCodes = brand?.currencies.map(({ code }) => code)
  brandGroups = brandGroups.map(({ betLimitGameProviderPairs, ...group }) => {
    let gameProviderBetLimits = Object.entries(
      betLimitGameProviderPairs
    ).reduce((gameProviderBetLimits, [brandBetLimitId, gameProviderPairs]) => {
      let list = Object.entries(gameProviderPairs).map(
        ([gameProviderId, gameProviderBetLimitId]) => {
          return {
            brandBetLimitId: parseInt(brandBetLimitId),
            gameProviderBetLimitId
          }
        }
      )

      return [...gameProviderBetLimits, ...list]
    }, [])

    return {
      ...group,
      gameProviderBetLimits
    }
  })

  return {
    ...brand,
    currencyCodes,
    brandGroups
  }
}

// brandBetLimitId -> gameProviderId -> gameProviderBetLimitId
const groupByBetLimit = (gameProviderBetLimits = []) => {
  let result = gameProviderBetLimits.reduce((result, gameProviderBetLimit) => {
    let {
      brandBetLimitId,
      gameProviderId,
      gameProviderBetLimitId
    } = gameProviderBetLimit

    let gameProviderBetLimitPairs = result[brandBetLimitId]
    if (!gameProviderBetLimitPairs) {
      gameProviderBetLimitPairs = {}
      result[brandBetLimitId] = gameProviderBetLimitPairs
    }

    gameProviderBetLimitPairs[gameProviderId] = gameProviderBetLimitId
    return result
  }, {})
  return result
}

const EditBody = ({ brand = {}, betLimits = [], gameProviders = [] }) => {
  let { control, setValue, getValues, watch, register } = useFormContext()
  let name = "brandGroups"
  const { fields, append, remove } = useFieldArray({
    control,
    name,
    keyName: "_id"
  })

  return (
    <Stack>
      {fields.map((field, index) => (
        <GroupCard
          key={field._id}
          name={name}
          groupField={field}
          groupIndex={index}
          betLimits={betLimits}
          gameProviders={gameProviders}
        />
      ))}
    </Stack>
  )
}

const GroupCard = ({
  name = "",
  groupField = {},
  groupIndex,
  betLimits = [],
  gameProviders = []
}) => {
  let { control, setValue } = useFormContext()
  let dialog = useModalState()
  let sportLabel = useTrans({
    zh: "体育",
    en: "Sport"
  })
  return (
    <Card variant="bordered">
      <Input
        name={`${name}[${groupIndex}].id`}
        value={groupField.id}
        type="hidden"
      />
      <Set alignX="right">
        <Switch
          name={`${name}[${groupIndex}].isSport`}
          label={sportLabel}
          defaultValue={groupField.isSport}
        />
        <Controller
          control={control}
          name={`${name}[${groupIndex}].brandGames`}
          defaultValue={groupField.brandGames}
          render={(
            { onChange, onBlur, value, name, ref },
            { invalid, isTouched, isDirty }
          ) => (
            <>
              <Disclosure {...dialog}>TODO: game list</Disclosure>
              {dialog.visible && (
                <GroupGameListDialog
                  gameProviders={gameProviders}
                  value={value}
                  dialog={dialog}
                  onChange={(data) => {
                    setValue(name, data)
                  }}
                />
              )}
            </>
          )}
        />
      </Set>
      <FieldStack>
        <InputField
          name={`${name}[${groupIndex}].name`}
          label={<Trans zh="名称" en="Name" />}
          defaultValue={groupField.name}
        />
        <TextareaField
          name={`${name}[${groupIndex}].description`}
          label={<Trans zh="说明" en="Description" />}
          defaultValue={groupField.description}
        />
        <BetLimitTable
          name={`${name}[${groupIndex}].betLimitGameProviderPairs`}
          group={groupField}
          groupIndex={groupIndex}
          betLimits={betLimits}
          gameProviders={gameProviders}
        />
      </FieldStack>
    </Card>
  )
}

const BetLimitTable = ({
  name,
  group = {},
  betLimits = [],
  gameProviders = []
}) => {
  return (
    <Table>
      <Table.Head>
        <Table.Row>
          <Table.HeadCell textAlign="center">
            <Trans zh="币种" en="Currency" />
          </Table.HeadCell>
          <Table.HeadCell textAlign="center">
            <Trans zh="最低投注" en="Min Bet" />
          </Table.HeadCell>
          <Table.HeadCell textAlign="center">
            <Trans zh="最高投注" en="Max Bet" />
          </Table.HeadCell>
          <Table.HeadCell textAlign="center">
            <Trans zh="单场最高投注" en="Max Bet Per Match" />
          </Table.HeadCell>
          <Table.HeadCell textAlign="center">
            <Trans zh="设定" en="Configuration" />
          </Table.HeadCell>
        </Table.Row>
      </Table.Head>
      <Table.Body>
        {betLimits.map((betLimit) => (
          <BetLimitRow
            name={`${name}.${betLimit.id}`}
            key={betLimit.id}
            group={group}
            betLimit={betLimit}
            gameProviders={gameProviders}
          />
        ))}
      </Table.Body>
    </Table>
  )
}

const BetLimitRow = ({
  name,
  group = {},
  betLimit = {},
  gameProviders = []
}) => {
  let { control, getValues, watch, setValue } = useFormContext()
  let dialog = useModalState()

  let gameProviderBetLimitPairs =
    group["betLimitGameProviderPairs"][betLimit.id]
  console.log("gameProviderBetLimitPairs", gameProviderBetLimitPairs)
  return (
    <Table.Row>
      <Table.Cell textAlign="center">{betLimit?.currencyCode}</Table.Cell>
      <Table.Cell textAlign="center">{betLimit?.minBet}</Table.Cell>
      <Table.Cell textAlign="center">{betLimit?.maxBet}</Table.Cell>
      <Table.Cell textAlign="center">{betLimit?.maxBetPerMatch}</Table.Cell>
      <Table.Cell textAlign="center">
        <Controller
          name={name}
          control={control}
          defaultValue={gameProviderBetLimitPairs}
          render={(
            { onChange, onBlur, value, name, ref },
            { invalid, isTouched, isDirty }
          ) => {
            return (
              <>
                <ConfigIconDisclosure {...dialog} />
                {dialog.visible && (
                  <BetLimitDialog
                    group={group}
                    betLimit={betLimit}
                    gameProviders={gameProviders}
                    dialog={dialog}
                    value={value}
                    onChange={(data) => {
                      setValue(name, data)
                    }}
                  />
                )}
              </>
            )
          }}
        />
      </Table.Cell>
    </Table.Row>
  )
}

const GroupGameListDialog = ({
  dialog,
  gameProviders = [],
  value: selectedGames = [],
  onChange,
  ...props
}) => {
  let title = useTrans({
    zh: "游戏列表",
    en: "Game List"
  })

  gameProviders = gameProviders.map((gameProvider) => {
    let games = gameProvider.games.map((gameProviderGame) => {
      let checked = !!selectedGames.find((game) => {
        return game.gameId === gameProviderGame.gameId
      })
      return {
        ...gameProviderGame,
        checked
      }
    })
    return {
      ...gameProvider,
      games
    }
  })

  return (
    <Backdrop {...dialog}>
      <FormDialog
        stopPropagation={true}
        defaultValues={{ gameProviders }}
        onSubmit={(pairs) => {
          let allCheckedGames = Object.entries(pairs).reduce(
            (allCheckedGames, [gameProviderId, games]) => {
              let checkedGames = games.filter(({ checked }) => checked)
              return [...allCheckedGames, ...checkedGames]
            },
            []
          )
          onChange?.(allCheckedGames)
          dialog.hide()
        }}
        dialog={dialog}
        title={title}
      >
        <Table>
          <Table.Head>
            <Table.Row>
              <Table.HeadCell></Table.HeadCell>
              <Table.HeadCell>TODO: Name</Table.HeadCell>
              <Table.HeadCell>TODO: Game List</Table.HeadCell>
            </Table.Row>
          </Table.Head>
          <Table.Body>
            {gameProviders.map((gameProvider, index) => (
              <GameProviderRow
                key={gameProvider.id}
                gameProvider={gameProvider}
              />
            ))}
          </Table.Body>
        </Table>
      </FormDialog>
    </Backdrop>
  )
}

const GameProviderRow = ({ gameProvider }) => {
  let { control, setValue } = useFormContext()
  let dialog = useModalState()
  return (
    <Table.Row>
      <Table.Cell>TODO: toogle game provider games</Table.Cell>
      <Table.Cell>{gameProvider.name}</Table.Cell>
      <Table.Cell>
        <Controller
          name={`${gameProvider.gameProviderId}`}
          defaultValue={gameProvider.games}
          control={control}
          render={(
            { onChange, onBlur, value, name, ref },
            { invalid, isTouched, isDirty }
          ) => {
            return (
              <>
                <Disclosure {...dialog}>TODO: games</Disclosure>
                {dialog.visible && (
                  <GameProviderGameListDialog
                    gameProviderId={gameProvider.gameProviderId}
                    value={value}
                    dialog={dialog}
                    onChange={(data) => {
                      setValue(name, data)
                    }}
                  />
                )}
              </>
            )
          }}
        />
      </Table.Cell>
    </Table.Row>
  )
}

const GameProviderGameListDialog = ({
  gameProviderId,
  dialog,
  value: gameProviderGames = [],
  onChange
}) => {
  let { data, isLoading, error } = useGameProvider(gameProviderId)

  if (isLoading) {
    return <LoadingBackdrop {...dialog} />
  }

  if (error) {
    return <AlertDialog {...error} dialog={dialog} />
  }

  let games = gameProviderGames.map((game) => {
    let record = data.games.find((item) => item.id === game.gameId)
    return {
      ...record,
      ...game
    }
  })

  return (
    <Backdrop {...dialog}>
      <FormDialog
        defaultValues={{
          games
        }}
        stopPropagation={true}
        onSubmit={(gameIdCheckedPairs) => {
          let list = gameProviderGames.map((game) => ({
            ...game,
            checked: gameIdCheckedPairs[game.gameId]
          }))
          onChange(list)
          dialog.hide()
        }}
        dialog={dialog}
      >
        <GameProviderGameListDialogBody />
      </FormDialog>
    </Backdrop>
  )
}

const GameProviderGameListDialogBody = ({}) => {
  let { control } = useFormContext()
  let { fields: gameFields } = useFieldArray({
    control,
    name: "games",
    keyName: "_id"
  })

  return (
    <Table>
      <Table.Head>
        <Table.Row>
          <Table.HeadCell></Table.HeadCell>
          <Table.HeadCell>TODO: Id</Table.HeadCell>
          <Table.HeadCell>TODO: Name</Table.HeadCell>
          <Table.HeadCell>TODO: Platform</Table.HeadCell>
        </Table.Row>
      </Table.Head>
      <Table.Body>
        {gameFields.map((gameField, index) => (
          <GameProviderGameRow
            key={gameField._id}
            gameField={gameField}
            index={index}
          />
        ))}
      </Table.Body>
    </Table>
  )
}

const GameProviderGameRow = ({ gameField, index }) => {
  return (
    <Table.Row>
      <Table.Cell>
        <Checkbox name={`${gameField.id}`} defaultChecked={gameField.checked} />
      </Table.Cell>
      <Table.Cell>{gameField.id}</Table.Cell>
      <Table.Cell>{gameField.name}</Table.Cell>
      <Table.Cell>{gameField.platformType}</Table.Cell>
    </Table.Row>
  )
}

const BetLimitDialog = ({
  group = {},
  betLimit = {},
  gameProviders = [],
  value: gameProviderBetLimitPairs = {},
  onChange,
  dialog
}) => {
  let gameProviderIds = gameProviders.map(
    ({ gameProviderId }) => gameProviderId
  )
  let { items = [], isLoading, error } = useGameProviderBetLimitList({
    betLimitId: betLimit.id,
    gameProviderIds,
    isSports: group.isSports,
    showAll: true
  })

  let title = useTrans({
    zh: "游戏商限红",
    en: "Game Provider Bet Limit"
  })
  if (isLoading) {
    return <LoadingBackdrop {...dialog} />
  }

  if (error) {
    return <AlertDialog {...error} onClickClose={dialog.hide} />
  }

  // gameProviderId -> gameProviderBetLimitList
  let gameProviderBetLimitListPairs = items.reduce((pairs, item) => {
    let gameProviderId = item.gameProviderId
    let gameProviderBetLimits = pairs[item.gameProviderId]
    if (!gameProviderBetLimits) {
      gameProviderBetLimits = []
    }
    gameProviderBetLimits.push(item)
    pairs[gameProviderId] = gameProviderBetLimits

    return pairs
  }, {})

  return (
    <Backdrop {...dialog}>
      <FormDialog
        stopPropagation={true}
        defaultValues={gameProviderBetLimitPairs}
        onSubmit={(data) => {
          onChange?.(data)
          dialog.hide()
        }}
        hideBackdrop={true}
        dialog={dialog}
        title={title}
      >
        <Table>
          <Table.Head>
            <Table.Row>
              <Table.HeadCell textAlign="center">
                <Trans zh="游戏商" en="Game Provider" />
              </Table.HeadCell>
              <Table.HeadCell textAlign="center">
                <Trans zh="限红" en="Bet Limit" />
              </Table.HeadCell>
            </Table.Row>
          </Table.Head>
          <Table.Body>
            {gameProviders.map((gameProvider) => {
              return (
                <GameProviderBetLimitsRow
                  key={gameProvider.gameProviderId}
                  group={group}
                  betLimit={betLimit}
                  gameProvider={gameProvider}
                  gameProviderBetLimits={
                    gameProviderBetLimitListPairs[gameProvider.gameProviderId]
                  }
                />
              )
            })}
          </Table.Body>
        </Table>
      </FormDialog>
    </Backdrop>
  )
}

const GameProviderBetLimitsRow = ({
  gameProvider = {},
  gameProviderBetLimits = []
}) => {
  let locale = useLocale()
  let options = gameProviderBetLimits?.map((gameProviderBetLimit) => {
    return {
      label: betLimitLabel({ ...gameProviderBetLimit, locale }),
      value: gameProviderBetLimit.id
    }
  })

  let name = `${gameProvider?.gameProviderId}`
  return (
    <Table.Row>
      <Table.Cell>{gameProvider?.name}</Table.Cell>
      <Table.Cell textAlign="center">
        {options.length > 0 && <Select name={name} options={options} />}
        {options.length == 0 && `--`}
      </Table.Cell>
    </Table.Row>
  )
}

const betLimitLabel = ({ minBet, maxBet, maxBetPerNatch, locale }) => {
  let minBetLabel = getTrans(
    {
      zh: "最低投注",
      en: "Min Bet"
    },
    locale
  )

  let maxBetLabel = getTrans(
    {
      zh: "最高投注",
      en: "Max Bet"
    },
    locale
  )

  let maxBetPerMatchLabel = getTrans(
    {
      zh: "单场最高投注",
      en: "Max Bet Per Match"
    },
    locale
  )

  let labels = [
    Number.isFinite(minBet) ? `${minBetLabel}: ${minBet}` : "",
    Number.isFinite(maxBet) ? `${maxBetLabel}: ${maxBet}` : "",
    Number.isFinite(maxBetPerNatch)
      ? `${maxBetPerMatchLabel}: ${maxBetPerNatch}`
      : ""
  ]
  return labels.filter((value) => !!value).join(", ")
}
