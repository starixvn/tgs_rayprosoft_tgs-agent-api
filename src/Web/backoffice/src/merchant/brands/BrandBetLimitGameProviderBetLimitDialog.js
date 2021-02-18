import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import {
  useModalState,
  Stack,
  FieldStack,
  Card,
  Set,
  Table,
  Button,
  DropdownMenu,
  Icon
} from "bumbag"
import { useIsDisplayMenuByRoute, useUserLicenseeId } from "../../user/context"
import { Disclosure, ConfigIconDisclosure } from "../../dialog/Disclosure"
import FormDialog from "../../form/FormDialog"
import { Update as UpdateMenu } from "./menu"
import { useUpdateBrand } from "./client"
import { getTrans, Trans, useLocale, useTrans } from "../../i18n"
import ConfirmButtonFooter, {
  CloseButtonFooter
} from "../../form/ConfirmButtonFooter"
import { useFormContext, useFieldArray, Controller } from "react-hook-form"
import LoadingBackdrop from "../../dialog/LoadingBackdrop"
import AlertDialog from "../../dialog/Alert"
import { Input, InputField } from "../../input/Input"
import { TextareaField } from "../../textarea"
import useLicenseeBrand from "./useLicenseeBrand"
import Backdrop from "../../dialog/Backdrop"
import { useGameProviderBetLimitList } from "../../game/providers/betlimits/client"
import { Select } from "../../select/Select"
import { useGameProvider } from "../../game/providers/client"
import { Checkbox } from "../../checkbox"
import cloneDeep from "clone-deep"
import { SportsSwitch } from "./SportsSwitch"
import { isSportGameType } from "../../game/GameType"
import { faEllipsisV } from "@fortawesome/free-solid-svg-icons"
import { filterBySameProp, filterChecked } from "../../list/filter"
import { pluckCode } from "../../list/pluck"
import { sortByNumberProp } from "../../list/sort"
import { CloseText, ConfirmText } from "../../button/ActionButtons"

export const EditBrandBetLimitGameProviderBetLimit = ({
  record,
  onSubmitted,
  ...props
}) => {
  let dialog = useModalState()

  let editable = useIsDisplayMenuByRoute([UpdateMenu])
  let readonly = !editable

  return (
    <>
      <Disclosure {...dialog}>
        <Trans zh="限红组" en="Bet Limit Group" />
      </Disclosure>
      {editable && dialog.visible && (
        <EditDialog
          brandId={record?.id}
          brandCode={record?.code}
          licenseeId={record?.licenseeId}
          onSubmitted={onSubmitted}
          dialog={dialog}
        />
      )}
      {readonly && dialog.visible && (
        <ViewDialog
          brandId={record?.id}
          brandCode={record?.code}
          licenseeId={record?.licenseeId}
          dialog={dialog}
        />
      )}
    </>
  )
}

const useDialogTitle = (brandCode = "") =>
  useTrans({
    zh: `${brandCode} 限红组`,
    en: `${brandCode} Bet Limit Group`
  })

const ViewDialog = ({
  brandId = "",
  brandCode = "",
  licenseeId = "",
  dialog
}) => {
  let {
    brand,
    gameProviders,
    betLimits,
    isLoading: isLoadingBrand,
    error: brandError,
    mutateBrand,
    deleteBrandCache
  } = useLicenseeBrand({ brandId, licenseeId })

  let title = useDialogTitle(brandCode)
  if (isLoadingBrand) {
    return <LoadingBackdrop {...dialog} />
  }

  if (brandError) {
    return (
      <AlertDialog {...brandError} {...dialog} onClickClose={dialog.hide} />
    )
  }

  const reset = () => {
    mutateBrand(undefined, false)
    deleteBrandCache()
  }

  betLimits = filterChecked(betLimits)

  gameProviders = groupGameProviderGames(
    filterChecked(gameProviders),
    brand?.games
  )

  return (
    <FormDialog
      defaultValues={{
        ...brand,
        brandGroups: toBrandGroupsFormData(brand?.brandGroups)
      }}
      onClickClose={reset}
      title={title}
      showActionButtons={false}
      footer={<CloseButtonFooter dialog={dialog} onClickClose={reset} />}
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
        readonly={true}
      />
    </FormDialog>
  )
}

const EditDialog = ({
  brandId = "",
  brandCode = "",
  licenseeId = "",
  onSubmitted,
  dialog
}) => {
  let {
    brand,
    gameProviders,
    betLimits,
    isLoading: isLoadingBrand,
    error: brandError,
    mutateBrand,
    deleteBrandCache
  } = useLicenseeBrand({ brandId, licenseeId })

  let [formData, setSubmit] = useState()
  let { result, isLoading, error, mutate } = useUpdateBrand(brandId, formData)

  let title = useDialogTitle(brandCode)

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

  betLimits = filterChecked(betLimits)

  gameProviders = groupGameProviderGames(
    filterChecked(gameProviders),
    brand?.games
  )

  return (
    <FormDialog
      defaultValues={{
        ...brand,
        brandGroups: toBrandGroupsFormData(brand?.brandGroups)
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

const groupGameProviderGames = (gameProviders = [], gamePool = []) =>
  gameProviders.map((gameProvider) => {
    let games = filterBySameProp(
      ["gameProviderId", gameProvider.gameProviderId],
      gamePool
    )

    return {
      ...gameProvider,
      games
    }
  })

const toBrandGroupsFormData = (brandGroups = []) => {
  return (
    brandGroups.reduce((result, brandGroup, index) => {
      let { gameProviderBetLimits } = brandGroup
      let betLimitGameProviderPairs = mapBetLimitGameProviderPair(
        gameProviderBetLimits
      )
      result.push({
        ...brandGroup,
        betLimitGameProviderPairs
      })
      return result
    }, []) ?? []
  )
}

const normalizeFormData = ({
  brand: { createdOn, updatedOn, currencies, ...brand } = {},
  brandGroups = [],
  ...data
}) => {
  brandGroups = brandGroups.map(
    ({ betLimitGameProviderPairs, brandGames = [], ...group }) => {
      let gameProviderBetLimits = toGameProviderBetLimits(
        betLimitGameProviderPairs
      )
      return {
        ...group,
        brandGames,
        gameProviderBetLimits
      }
    }
  )

  return {
    ...brand,
    currencyCodes: pluckCode(currencies),
    brandGroups
  }
}

// { brandBetLimitId: { gameProviderId: gameProviderBetLimitId } } -> [ { brandBetLimitId, gameProviderBetLimitId } ]
const toGameProviderBetLimits = (betLimitGameProviderPairs = {}) =>
  Object.entries(betLimitGameProviderPairs).reduce(
    (
      gameProviderBetLimits,
      [brandBetLimitIdString, gameProviderPairs = {}]
    ) => [
      ...gameProviderBetLimits,
      ...toBetLimits(brandBetLimitIdString, gameProviderPairs)
    ],
    []
  )

// gameProviderPairs: { gameProviderId: gameProviderBetLimitId }
const toBetLimits = (brandBetLimitIdString = "", gameProviderPairs = {}) => {
  let brandBetLimitId = parseInt(brandBetLimitIdString)

  return Object.entries(gameProviderPairs).map(
    ([gameProviderId, gameProviderBetLimitId]) => ({
      brandBetLimitId,
      gameProviderBetLimitId
    })
  )
}

// brandBetLimitId -> gameProviderBetLimitPairs: { gameProviderId: gameProviderBetLimitId }
const mapBetLimitGameProviderPair = (gameProviderBetLimits = []) => {
  let result = gameProviderBetLimits.reduce(
    (result, { brandBetLimitId, gameProviderId, gameProviderBetLimitId }) => {
      let gameProviderBetLimitPairs = result[brandBetLimitId]
      if (!gameProviderBetLimitPairs) {
        gameProviderBetLimitPairs = {}
        result[brandBetLimitId] = gameProviderBetLimitPairs
      }

      gameProviderBetLimitPairs[gameProviderId] = gameProviderBetLimitId
      return result
    },
    {}
  )
  return result
}

const EditBody = ({
  brand = {},
  betLimits = [],
  gameProviders = [],
  readonly = false
}) => {
  let { control, setValue, getValues, watch, register } = useFormContext()
  let name = "brandGroups"
  const { fields, append, remove } = useFieldArray({
    control,
    name,
    keyName: "_id"
  })

  let isEmpty = fields.length === 0
  let editable = !readonly
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
          removeGroup={remove}
          readonly={readonly}
        />
      ))}
      {readonly && isEmpty && (
        <Card variant="bordered" alignX="center">
          <Trans zh="查无资料" en="No Result" />
        </Card>
      )}
      {editable && <AddGroupCard appendGroup={append} />}
    </Stack>
  )
}

const AddGroupCard = ({ appendGroup, gameProviderBetLimits = [] }) => {
  let [isSports, setSports] = useState(false)
  return (
    <Card variant="bordered" alignY="center">
      <Set alignX="center">
        <SportsSwitch
          checked={isSports}
          onChange={(event) => {
            let checked = event.target.checked
            setSports(checked)
          }}
        />
        <Button
          onClick={(event) => {
            appendGroup({
              isSports,
              betLimitGameProviderPairs: []
            })
          }}
        >
          <Trans zh="新增" en="Add" />
        </Button>
      </Set>
    </Card>
  )
}

const GroupCard = ({
  name = "",
  groupField = {},
  groupIndex,
  betLimits = [],
  gameProviders = [],
  removeGroup,
  readonly = false
}) => {
  let { control, setValue, getValues } = useFormContext()
  let dialog = useModalState()

  let hasGroupId = !!groupField?.id

  let isSportsFieldName = `${name}[${groupIndex}].isSports`
  let editable = !readonly
  return (
    <Card variant="bordered">
      {hasGroupId && (
        <Input
          name={`${name}[${groupIndex}].id`}
          value={groupField.id}
          type="hidden"
        />
      )}
      <Set alignX="right">
        <SportsSwitch
          name={isSportsFieldName}
          defaultChecked={groupField.isSports}
          disabled
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
              <Disclosure {...dialog}>
                <Trans zh="游戏列表" en="Game List" />
              </Disclosure>
              {dialog.visible && (
                <GroupGameListDialog
                  isSportsFieldName={isSportsFieldName}
                  getValues={getValues}
                  gameProviders={gameProviders}
                  value={value}
                  readonly={readonly}
                  dialog={dialog}
                  onChange={(data) => {
                    if (editable) {
                      setValue(name, data)
                    }
                  }}
                />
              )}
            </>
          )}
        />
        {editable && (
          <DropdownMenu
            menu={
              <DropdownMenu.Item
                iconBefore="solid-trash-alt"
                color="danger"
                onClick={() => {
                  removeGroup?.(groupIndex)
                }}
              >
                <Trans zh="删除" en="Delete" />
              </DropdownMenu.Item>
            }
          >
            <Button>
              <Icon icon={faEllipsisV} type="font-awesome" />
            </Button>
          </DropdownMenu>
        )}
      </Set>
      <FieldStack>
        <FieldStack orientation="horizontal">
          <InputField
            name={`${name}[${groupIndex}].name`}
            label={<Trans zh="名称" en="Name" />}
            defaultValue={groupField.name}
            disabled={readonly}
          />
          <InputField
            name={`${name}[${groupIndex}].code`}
            label={<Trans zh="代码" en="Code" />}
            defaultValue={groupField.code}
            disabled={readonly}
          />
        </FieldStack>
        <TextareaField
          name={`${name}[${groupIndex}].description`}
          label={<Trans zh="说明" en="Description" />}
          defaultValue={groupField.description}
          disabled={readonly}
        />
        <BetLimitTable
          name={`${name}[${groupIndex}].betLimitGameProviderPairs`}
          group={groupField}
          groupIndex={groupIndex}
          betLimits={betLimits}
          gameProviders={gameProviders}
          readonly={readonly}
        />
      </FieldStack>
    </Card>
  )
}

const BetLimitTable = ({
  name,
  group = {},
  betLimits = [],
  gameProviders = [],
  readonly = false
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
            readonly={readonly}
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
  gameProviders = [],
  readonly = false
}) => {
  let { control, getValues, watch, setValue } = useFormContext()
  let dialog = useModalState()

  let gameProviderBetLimitPairs =
    group["betLimitGameProviderPairs"][betLimit.id] ?? {}
  let editable = !readonly
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
                    readonly={readonly}
                    onChange={(data) => {
                      if (editable) {
                        setValue(name, data)
                      }
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
  isSportsFieldName,
  getValues,
  gameProviders = [],
  value: checkedGames = [],
  onChange,
  dialog,
  readonly = false,
  ...props
}) => {
  let title = useTrans({
    zh: "游戏列表",
    en: "Game List"
  })

  let isSports = getValues(isSportsFieldName)

  gameProviders = mapGroupGameProviderGames(
    isSports,
    checkedGames,
    gameProviders
  )

  let editable = !readonly
  return (
    <Backdrop {...dialog}>
      <FormDialog
        stopPropagation={true}
        defaultValues={{ gameProviders }}
        onSubmit={(gameProviderIdGamesPairs) => {
          if (editable) {
            let allCheckedGames = toCheckedGames(gameProviderIdGamesPairs)
            onChange?.(allCheckedGames)
          }
          dialog.hide()
        }}
        submitText={readonly ? <CloseText /> : <ConfirmText />}
        actionButtonsProps={{
          submitProps: {
            variant: "outlined"
          },
          addonButtons: editable && (
            <Button
              palette="primary"
              variant="outlined"
              onClick={(event) => {
                let allGames = gameProviders.reduce(
                  (allGames, { games = [] }) => [...allGames, ...games],
                  []
                )

                onChange?.(allGames)
                dialog.hide()
              }}
            >
              <Trans zh="全选" en="Select All" />
            </Button>
          )
        }}
        dialog={dialog}
        title={title}
        css={css`
          & .bb-DialogContent {
            position: relative;
          }
        `}
      >
        <Table>
          <Table.Head>
            <Table.Row>
              <Table.HeadCell textAlign="center">
                <Trans zh="游戏商" en="Game Provider Name" />
              </Table.HeadCell>
              <Table.HeadCell textAlign="center"></Table.HeadCell>
            </Table.Row>
          </Table.Head>
          <Table.Body>
            {gameProviders.map((gameProvider, index) => (
              <GameProviderRow
                key={gameProvider.id}
                gameProvider={gameProvider}
                readonly={readonly}
              />
            ))}
          </Table.Body>
        </Table>
      </FormDialog>
    </Backdrop>
  )
}

const toCheckedGames = (gameProviderIdGamesPairs = {}) => {
  return Object.entries(gameProviderIdGamesPairs).reduce(
    (allCheckedGames, [gameProviderId, games]) => [
      ...allCheckedGames,
      ...filterChecked(games)
    ],
    []
  )
}

const mapGroupGameProviderGames = (
  groupOfSport = false,
  checkedGames = [],
  gameProviders = []
) => {
  return gameProviders
    .map(({ games = [], ...gameProvider }) => {
      games = filterGameProviderGames(groupOfSport, games).map(
        (gameProviderGame) => {
          let checked = !!checkedGames.find((game) => {
            return game.gameId === gameProviderGame.gameId
          })
          return {
            ...gameProviderGame,
            checked
          }
        }
      )

      return {
        ...gameProvider,
        games: sortByNumberProp("gameId", games)
      }
    })
    .filter(({ games = [], ...gameProvider }) => games.length > 0)
}

const filterGameProviderGames = (
  groupOfSport = false,
  gameProviderGames = []
) =>
  gameProviderGames.filter(({ gameType }) => {
    let isSport = isSportGameType(gameType)
    if (groupOfSport) {
      return isSport
    }

    return !isSport
  })

const GameProviderRow = ({ gameProvider, readonly = false }) => {
  let { control, setValue } = useFormContext()
  let dialog = useModalState()
  let editable = !readonly
  return (
    <Table.Row>
      <Table.Cell textAlign="center">{gameProvider.name}</Table.Cell>
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
                <Disclosure {...dialog}>
                  <Trans zh="游戏列表" en="Game List" />
                </Disclosure>
                {dialog.visible && (
                  <GameProviderGameListDialog
                    gameProviderId={gameProvider.gameProviderId}
                    value={value}
                    readonly={readonly}
                    dialog={dialog}
                    onChange={(data) => {
                      if (editable) {
                        setValue(name, data)
                      }
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
  readonly = false,
  onChange
}) => {
  let { data: gameProvider, isLoading, error } = useGameProvider(gameProviderId)

  if (isLoading) {
    return <LoadingBackdrop {...dialog} />
  }

  if (error) {
    return <AlertDialog {...error} dialog={dialog} />
  }

  let games = gameProviderGames.map((gameField) => {
    let record =
      gameProvider.games.find((game) => game.id === gameField.gameId) ?? {}
    return {
      ...record,
      ...gameField
    }
  })

  let editable = !readonly
  return (
    <Backdrop {...dialog}>
      <FormDialog
        defaultValues={{
          games
        }}
        stopPropagation={true}
        onSubmit={(gameIdCheckedPairs) => {
          if (editable) {
            let list = gameProviderGames.map((game) => ({
              ...game,
              checked: gameIdCheckedPairs[game.gameId]
            }))

            onChange(list)
          }
          dialog.hide()
        }}
        submitText={readonly ? <CloseText /> : <ConfirmText />}
        actionButtonsProps={{
          submitProps: {
            variant: "outlined"
          },
          addonButtons: editable && (
            <Button
              palette="primary"
              variant="outlined"
              onClick={(event) => {
                let list = gameProviderGames.map((game) => ({
                  ...game,
                  checked: true
                }))

                onChange(list)
                dialog.hide()
              }}
            >
              <Trans zh="全选" en="Select All" />
            </Button>
          )
        }}
        dialog={dialog}
        title={gameProvider.name}
        css={css`
          & .bb-DialogContent {
            position: relative;
          }
        `}
      >
        <GameProviderGameListDialogBody readonly={readonly} />
      </FormDialog>
    </Backdrop>
  )
}

const GameProviderGameListDialogBody = ({ readonly = false }) => {
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
          <Table.HeadCell>
            <Trans zh="游戏ID" en="Game ID" />
          </Table.HeadCell>
          <Table.HeadCell>
            <Trans zh="游戏名称" en="Game Name" />
          </Table.HeadCell>
          <Table.HeadCell>
            <Trans zh="支持设备" en="Platform" />
          </Table.HeadCell>
        </Table.Row>
      </Table.Head>
      <Table.Body>
        {gameFields.map((gameField, index) => (
          <GameProviderGameRow
            key={gameField._id}
            gameField={gameField}
            readonly={readonly}
            index={index}
          />
        ))}
      </Table.Body>
    </Table>
  )
}

const GameProviderGameRow = ({ gameField, index, readonly = false }) => {
  return (
    <Table.Row>
      <Table.Cell>
        <Checkbox
          name={`${gameField.gameId}`}
          defaultChecked={gameField.checked}
          disabled={readonly}
        />
      </Table.Cell>
      <Table.Cell>{gameField.gameId}</Table.Cell>
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
  dialog,
  readonly = false
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

  let editable = !readonly
  return (
    <Backdrop {...dialog}>
      <FormDialog
        stopPropagation={true}
        defaultValues={gameProviderBetLimitPairs}
        onSubmit={(data) => {
          if (editable) {
            onChange?.(filterGameProviderBetLimitId(data))
          }
          dialog.hide()
        }}
        submitText={readonly ? <CloseText /> : <ConfirmText />}
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
              let gameProviderBetLimits =
                gameProviderBetLimitListPairs[gameProvider.gameProviderId]
              return (
                <GameProviderBetLimitsRow
                  key={gameProvider.gameProviderId}
                  group={group}
                  betLimit={betLimit}
                  gameProvider={gameProvider}
                  gameProviderBetLimits={gameProviderBetLimits}
                  readonly={readonly}
                />
              )
            })}
          </Table.Body>
        </Table>
      </FormDialog>
    </Backdrop>
  )
}

const filterGameProviderBetLimitId = (gameProviderIdBetLimitIdPairs = {}) =>
  Object.entries(gameProviderIdBetLimitIdPairs).reduce(
    (record, [gameProviderId, gameProviderBetLimitId = ""]) => {
      if (gameProviderBetLimitId) {
        Reflect.set(record, gameProviderId, gameProviderBetLimitId)
      }

      return record
    },
    {}
  )

const GameProviderBetLimitsRow = ({
  gameProvider = {},
  gameProviderBetLimits = [],
  readonly = false
}) => {
  let locale = useLocale()
  let options = gameProviderBetLimits?.map((gameProviderBetLimit) => {
    return {
      label: betLimitLabel({ ...gameProviderBetLimit, locale }),
      value: gameProviderBetLimit.id
    }
  })

  let name = `${gameProvider?.gameProviderId}`
  let gameProviderBetLimitPlaceholder = useTrans({
    zh: "选择游戏商限红",
    en: "Select Game Provider Bet Limit"
  })
  return (
    <Table.Row>
      <Table.Cell>{gameProvider?.name}</Table.Cell>
      <Table.Cell textAlign="center">
        {options.length > 0 && (
          <Select
            name={name}
            options={options}
            placeholder={gameProviderBetLimitPlaceholder}
            disabled={readonly}
          />
        )}
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
