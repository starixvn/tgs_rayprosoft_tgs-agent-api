import React, { useState, useEffect, useMemo } from "react"
import { css, Global } from "@emotion/react"
import { useForm, FormProvider, useFormContext } from "react-hook-form"
import { Heading, Stack, Box, Button, Set, useModalState } from "bumbag"
import PageContent from "../../page/Content"
import {
    useIsDisplayMenuByRoute,
    useHasSubMenuByRoute,
    useUserBrandId
} from "../../user/context"
import { useTable, Table, pageCount, toApiParams } from "../../table"
import { Trans, useTrans } from "../../i18n"
import SearchButton from "../../button/SearchButton"
import equals from "fast-deep-equal"
import { InputField } from "../../input/Input"
import { Code } from "./menu"
import BrandBar from "../BrandBar"
import { today, yesterday } from "../../date/range"
import { startOfNextDay } from "../../date/startOfNextDay"
import { textColumn } from "../../table/column/textColumn"
import dateTimeColumn from "../../table/column/dateTime"
import numberColumn, {
    numberDecimalColumn
} from "../../table/column/numberColumn"
import subDays from "date-fns/subDays"
import { DateInputRange } from "../../date/DateTimeRangeInput"
import {
    GameRoundStatusLabel,
    GameRoundStatusSelect
} from "../../game/round/GameRoundStatus"
import GameProviderSelect from "../../game/providers/GameProviderSelect"
import { useGameRoundList } from "../../game/round/client"
import { GameRoundButton } from "./button"
import AlertDialog from "../../dialog/Alert"

export { Code }

export const Content = () => {
    let isDisplay = useIsDisplayMenuByRoute()
    if (!isDisplay) {
        return null
    }

    return <Body />
}

export const CodeContentPair = [Code, Content]

export const Body = () => {
    let userBrandId = useUserBrandId()
    let [brandId, setBrandId] = useState(userBrandId)

    return (
        <Set orientation="vertical" isFilled>
            <BrandBar
                onBrandChange={(value) => {
                    setBrandId(value)
                }}
            />
            {brandId && <ReportContent brandId={brandId} />}
        </Set>
    )
}

const ReportContent = ({ brandId }) => {
    let dateRange = today()
    let QueryMaxRangeDays = 30
    let { state, setState, setPageIndex, setSortOrder } = useTable({
        brandId,
        from: dateRange.start,
        to: dateRange.end,
        username: "",
        roundId: "",
        status: "",
        gameProviderId: ""
    })

    useEffect(() => {
        setState({ pageIndex: 0, brandId })
    }, [brandId])

    let {
        pageIndex,
        pageSize,
        from,
        to,
        username,
        roundId,
        status,
        gameProviderId
    } = state

    let { isLoading, data = {}, error, mutate, deleteCache } = useGameRoundList(
        {
            ...toApiParams(state),
            sortby: "createdOn",
            orderby: "descending",
            from,
            to: startOfNextDay(to)
        }
    )

    let DataRetentionDays = 180
    let minDate = subDays(dateRange.start, DataRetentionDays)

    let columns = useColumns({ items: data?.items })
    return (
        <PageContent
            actionBar={
                <SearchBar
                    start={from}
                    end={to}
                    username={username}
                    roundId={roundId}
                    status={status}
                    gameProviderId={gameProviderId}
                    minDate={minDate}
                    maxDate={dateRange.end}
                    maxRangeDays={QueryMaxRangeDays}
                    isLoading={isLoading}
                    search={({ start, end, ...formData }) => {
                        let newState = {
                            pageIndex: 0,
                            from: start,
                            to: end,
                            username: formData.username,
                            roundId: formData.roundId,
                            status: formData.status,
                            gameProviderId: formData.gameProviderId
                        }

                        if (
                            equals(newState, {
                                pageIndex,
                                from: state?.from,
                                to: state?.to,
                                username,
                                roundId,
                                status,
                                gameProviderId
                            })
                        ) {
                            mutate()
                        }
                        setState(newState)
                    }}
                />
            }
            title={<Trans zh="订单查询" en="Order Tracking" />}
        >
            <Table
                controlledState={state}
                columns={columns}
                data={data?.items}
                pageCount={pageCount(pageSize, data?.totalCount)}
                setPageIndex={setPageIndex}
                setSortOrder={setSortOrder}
                isLoading={isLoading}
            />
            {error && (
                <AlertDialog
                    {...error}
                    onClickClose={() => {
                        deleteCache()
                        mutate({ data: undefined }, false)
                    }}
                />
            )}
        </PageContent>
    )
}

const SearchBar = ({
    start,
    end,
    username,
    roundId,
    status,
    gameProviderId,
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
            end,
            username,
            roundId,
            status,
            gameProviderId
        }
    })

    const onSubmit = (data) => search?.(data)

    let playerNamePlaceholder = useTrans({
        zh: "用户名",
        en: "Player Name"
    })

    let roundIdPlaceholder = useTrans({
        zh: "订单ID",
        en: "Round ID"
    })
    return (
        <FormProvider {...formMethods}>
            <form onSubmit={formMethods.handleSubmit(onSubmit)}>
                <Set>
                    <DateInputRange
                        minDate={minDate}
                        maxDate={maxDate}
                        maxRangeDays={maxRangeDays}
                        required
                    />
                    <InputField
                        name="username"
                        showEmptyFieldValueAddonAfter
                        placeholder={playerNamePlaceholder}
                        showEmptyFieldValueAddonAfter
                    />
                    <InputField
                        name="roundId"
                        showEmptyFieldValueAddonAfter
                        placeholder={roundIdPlaceholder}
                    />
                    <GameRoundStatusSelect name="status" />
                    {/* TODO: filter by brandId? or use BrandGameProviderSelect? */}
                    <GameProviderSelect name="gameProviderId" />
                    <SearchButton isLoading={isLoading} disabled={isLoading} />
                </Set>
            </form>
        </FormProvider>
    )
}

const useColumns = ({ items = [] }) => {
    return useMemo(
        () => [
            textColumn({
                accessor: "playerName",
                Header: <Trans zh="用户名" en="Player Name" />,
                disableSortBy: true
            }),
            textColumn({
                accessor: "externalRoundId",
                Header: <Trans zh="三方订单ID" en="External Id" />,
                disableSortBy: true
            }),
            textColumn({
                accessor: "id",
                Header: <Trans zh="订单ID" en="Id" />,
                disableSortBy: true
            }),
            dateTimeColumn({
                accessor: "createdOn",
                Header: <Trans zh="投注时间" en="Created On" />,
                disableSortBy: true
            }),
            dateTimeColumn({
                accessor: "closedOn",
                Header: <Trans zh="结算时间" en="Closed On" />,
                disableSortBy: true
            }),
            textColumn({
                accessor: "gameProviderCode",
                Header: <Trans zh="游戏厂商" en="Game Provider" />,
                disableSortBy: true
            }),
            textColumn({
                accessor: "gameName",
                Header: <Trans zh="游戏名称" en="Game Name" />,
                disableSortBy: true
            }),
            {
                accessor: "status",
                Header: <Trans zh="状态" en="Status" />,
                disableSortBy: true,
                Cell: GameRoundStatusLabel,
                headCell: {
                    textAlign: "center",
                    whiteSpace: "nowrap"
                },
                cell: {
                    textAlign: "center",
                    whiteSpace: "nowrap"
                }
            },
            textColumn({
                accessor: "currencyCode",
                Header: <Trans zh="币种" en="Currency" />,
                disableSortBy: true
            }),
            numberDecimalColumn({
                accessor: "betAmount",
                Header: <Trans zh="投注金额" en="Bet Amount" />,
                disableSortBy: true
            }),
            numberDecimalColumn({
                accessor: "validBetAmount",
                Header: <Trans zh="有效投注" en="Valid Bet Amount" />,
                disableSortBy: true
            }),
            numberDecimalColumn({
                accessor: "settleAmount",
                Header: <Trans zh="派彩" en="Settle Amount" />,
                disableSortBy: true
            }),
            numberDecimalColumn({
                accessor: "winLoss",
                Header: <Trans zh="输赢金额" en="Win Loss" />,
                disableSortBy: true
            }),
            {
                id: "detail",
                accessor: (record) => record,
                Header: <Trans zh="详情" en="Detail" />,
                disableSortBy: true,
                Cell: ({ value }) => {
                    return (
                        <Set>
                            <GameRoundButton record={value} />
                        </Set>
                    )
                },
                headCell: {
                    textAlign: "center",
                    whiteSpace: "nowrap"
                },
                cell: {
                    textAlign: "center",
                    whiteSpace: "nowrap"
                }
            }
        ],
        [JSON.stringify(items)]
    )
}
