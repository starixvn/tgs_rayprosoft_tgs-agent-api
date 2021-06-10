import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { Spinner, Stack, Text, Box, Button, Set, Tabs as _Tabs } from "bumbag"
import { Trans, FormattedNumber, useTrans } from "../../i18n"
import format from "../../date/format"
import { useTable, Table, pageCount, toApiParams } from "../../table"
//TODO: use BrandGameType instead?
import { GameTypeSelect } from "../../game/GameType"
//TODO: use BrandGameProviderSelect instead?
import GameProviderSelect from "../../game/providers/GameProviderSelect"
import { pastWeek, today } from "../../date/range"
import DateTimeRangeInput from "../../date/DateTimeRangeInput"
import SearchButton from "../../button/SearchButton"
import { useBettings } from "./client"
import equals from "fast-deep-equal"
import dateTimeColumn from "../../table/column/dateTime"
import BettingDetail from "./BettingDetailButton"
import { Tab as _Tab, TabPanel } from "../../tab"
import { useForm, FormProvider, useFormContext } from "react-hook-form"
import { sumColumn } from "../../table/sumColumn"
import { startOfNextDay } from "../../date/startOfNextDay"
import { subDays } from "date-fns"
import { useGameRoundList } from "../../game/round/client"
import numberColumn, {
    numberDecimalColumn
} from "../../table/column/numberColumn"
import AlertDialog from "../../dialog/Alert"

export const Key = "betting"

export const Tab = ({ ...props }) => {
    return (
        <_Tab tabId={Key} {...props}>
            <Trans zh="投注记录" en="Betting records" />
        </_Tab>
    )
}

export const Panel = ({ playerCode, brandId, player, ...props }) => {
    let { selectedId } = props
    let isActive = selectedId === Key
    let dateRange = today()
    let { state, setState, setPageIndex, setSortOrder } = useTable({
        gameType: "",
        gameProviderId: "",
        from: dateRange.start,
        to: dateRange.end
    })

    let { pageIndex, pageSize, from, to, gameType, gameProviderId } = state

    let {
        isLoading,
        data = {},
        error,
        mutate,
        deleteCache
    } = useGameRoundList(
        isActive
            ? {
                  ...toApiParams(state),
                  sortby: "createdOn",
                  orderby: "descending",
                  brandId,
                  username: player.username
              }
            : undefined
    )

    let columns = useColumns({ items: data?.items })

    let QueryMaxRangeDays = 30
    let DataRetentionDays = 180
    let minDate = subDays(dateRange.start, DataRetentionDays)

    return (
        <TabPanel tabId={Key} {...props}>
            <Stack spacing="major-2">
                <SearchBar
                    gameType={gameType}
                    gameProviderId={gameProviderId}
                    start={from}
                    end={to}
                    minDate={minDate}
                    maxDate={dateRange.end}
                    maxRangeDays={QueryMaxRangeDays}
                    search={({ start, end, ...formData }) => {
                        setState({
                            pageIndex: 0,
                            from: start,
                            to: end,
                            ...formData
                        })
                        mutate()
                    }}
                    isLoading={isLoading}
                />
                <Table
                    columns={columns}
                    data={data?.items}
                    pageCount={pageCount(pageSize, data?.totalCount)}
                    setPageIndex={setPageIndex}
                    setSortOrder={setSortOrder}
                    initialState={state}
                    controlledState={state}
                    isLoading={isLoading}
                    foot={data?.items?.length > 0}
                />
                {error && <AlertDialog {...error} onClickClose={deleteCache} />}
            </Stack>
        </TabPanel>
    )
}

const SearchBar = ({
    gameType,
    gameProviderId,
    start,
    end,
    minDate,
    maxDate,
    maxRangeDays,
    search,
    isLoading,
    ...props
}) => {
    let formMethods = useForm({
        defaultValues: {
            start,
            end
        }
    })

    const onSubmit = (data) => search(data)

    return (
        <FormProvider {...formMethods}>
            <form onSubmit={formMethods.handleSubmit(onSubmit)}>
                <Set>
                    <GameTypeSelect name="gameType" />
                    <GameProviderSelect name="gameProviderId" />
                    <DateTimeRangeInput
                        minDate={minDate}
                        maxDate={maxDate}
                        maxRangeDays={maxRangeDays}
                        required
                    />
                    <SearchButton isLoading={isLoading} disabled={isLoading} />
                </Set>
            </form>
        </FormProvider>
    )
}

const useColumns = ({ items = [] }) => {
    let roundIdTitle = useTrans({
        zh: "订单ID",
        en: "ID"
    })
    let gameIdTitle = useTrans({
        zh: "游戏ID",
        en: "Game ID"
    })
    return useMemo(
        () => [
            dateTimeColumn({
                accessor: "createdOn",
                Header: <Trans zh="投注时间" en="Created On" />,
                disableSortBy: true,
                Footer: () => <Trans zh="总和" en="Total" />,
                footerCellProps: {
                    textAlign: "center"
                }
            }),
            numberDecimalColumn({
                accessor: "betAmount",
                sumColumn: "betAmount",
                Header: <Trans zh="投注金额" en="Bet Amount" />,
                disableSortBy: true
            }),
            numberDecimalColumn({
                accessor: "validBetAmount",
                sumColumn: "validBetAmount",
                Header: <Trans zh="有效投注金额" en="Valid Bet Amount" />,
                disableSortBy: true
            }),
            numberDecimalColumn({
                accessor: "settleAmount",
                sumColumn: "settleAmount",
                Header: <Trans zh="派彩" en="Settle Amount" />,
                disableSortBy: true
            }),
            numberDecimalColumn({
                accessor: "winLoss",
                Header: <Trans zh="输赢金额" en="Win Loss" />,
                sumColumn: "winLoss",
                disableSortBy: true,
                Cell: ({ value }) => {
                    let win = value >= 0
                    return (
                        <Text color={win ? "success" : "danger"}>
                            <FormattedNumber value={value} format="+0,0.00" />
                        </Text>
                    )
                }
            }),
            {
                id: "detail",
                accessor: (row) => row,
                Header: <Trans zh="游戏详情" en="Game Detail" />,
                disableSortBy: true,
                headCell: {
                    textAlign: "center"
                },
                cell: {
                    textAlign: "center",
                    whiteSpace: "nowrap"
                },
                Cell: ({ value }) => {
                    let { id, gameId = "" } = value
                    return (
                        <Text>
                            <span title={roundIdTitle}>{id}</span> |
                            <span title={gameIdTitle}>{gameId}</span>
                        </Text>
                    )
                }
            },
            {
                id: "actions",
                disableSortBy: true,
                Header: <Trans zh="操作" en="Actions" />,
                headCell: {
                    textAlign: "center"
                },
                cell: {
                    textAlign: "center"
                },
                accessor: (row) => {
                    return row
                },
                Cell: ({ value }) => {
                    return (
                        <BettingDetail record={value}>
                            <Trans zh="详情" en="Detail" />
                        </BettingDetail>
                    )
                }
            }
        ],
        [JSON.stringify(items)]
    )
}
